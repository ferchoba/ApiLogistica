using Logistica.Domain.Entities;
using Logistica.Domain.Interfaces;
using Logistica.Domain.Rules;
using Logistica.Domain.Constants;
using Logistica.Application.Dtos;
using Logistica.Application.Resources;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Logistica.Application.Services;

public class NormalizationOrchestratorService(
    IDeliveryRepository repository,
    ILogger<NormalizationOrchestratorService> logger) : INormalizationOrchestrator
{
    public async Task<NormalizationResponse> ProcessFileAsync(
        Stream fileStream,
        IDeliveryParser parser,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(AppMessages.Log_OrchestrationStarted, parser.FormatId);

        var channelOptions = new BoundedChannelOptions(LogisticaConstants.DEFAULT_CHANNEL_CAPACITY)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = true,
            SingleReader = true
        };
        var channel = Channel.CreateBounded<(DeliveryOrder? Order, DeliveryError? Error)>(channelOptions);

        var producerTask = ProduceDataAsync(fileStream, parser, channel.Writer, cancellationToken);
        var consumerTask = ConsumeAndProcessDataAsync(channel.Reader, cancellationToken);

        await Task.WhenAll(producerTask, consumerTask);

        var response = await consumerTask;

        logger.LogInformation(AppMessages.Log_OrchestrationEnded, response.Summary.TotalProcessed, response.Summary.TotalSuccessful, response.Summary.TotalFailed);

        return response;
    }

    private static async Task ProduceDataAsync(
        Stream stream,
        IDeliveryParser parser,
        ChannelWriter<(DeliveryOrder? Order, DeliveryError? Error)> writer,
        CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var item in parser.ParseAsync(stream, cancellationToken))
            {
                await writer.WriteAsync(item, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            await writer.WriteAsync((null, new DeliveryError(0, "N/A", "FATAL_READ_ERROR", ex.Message)), cancellationToken);
        }
        finally
        {
            writer.Complete();
        }
    }

    private async Task<NormalizationResponse> ConsumeAndProcessDataAsync(
        ChannelReader<(DeliveryOrder? Order, DeliveryError? Error)> reader,
        CancellationToken cancellationToken)
    {
        int limit = LogisticaConstants.MAX_NORMALIZATION_RECORDS;
        HashSet<string> processedIds = new(limit); 
        List<DeliveryError> errors = new(limit);
        List<DeliveryOrder> normalizedData = new(limit); 

        int totalProcessed = 0;
        int totalSuccessful = 0;
        int totalFailed = 0;

        await foreach ((DeliveryOrder? orderItem, DeliveryError? errorItem) in reader.ReadAllAsync(cancellationToken))
        {
            totalProcessed++;

            if (errorItem != null)
            {
                errors.Add(errorItem);
                totalFailed++;
                continue;
            }

            DeliveryOrder order = orderItem!;

            if (!processedIds.Add(order.OrderId))
            {
                errors.Add(new DeliveryError(totalProcessed, order.OrderId, "DUPLICATE_ORDER", AppMessages.Validation_DuplicateOrder));
                totalFailed++;
                continue;
            }

            List<DeliveryError> validationErrors = OrderValidator.Validate(order, totalProcessed).ToList();
            if (validationErrors.Count != 0)
            {
                errors.AddRange(validationErrors);
                totalFailed++;
                continue;
            }

            normalizedData.Add(order);
            totalSuccessful++;
        }

        if (normalizedData.Count != 0)
        {
            logger.LogInformation(AppMessages.Log_DelegatingPersistence, normalizedData.Count);
            await repository.BulkInsertAsync(normalizedData, cancellationToken);
        }

        var summary = new ProcessingSummary(totalProcessed, totalSuccessful, totalFailed);
        return new NormalizationResponse(summary, errors, normalizedData);
    }
}
