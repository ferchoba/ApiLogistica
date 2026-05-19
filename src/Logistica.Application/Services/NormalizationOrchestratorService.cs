using Logistica.Domain.Entities;
using Logistica.Domain.Interfaces;
using Logistica.Domain.Rules;
using Logistica.Domain.Constants;
using Logistica.Application.Dtos;
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
        logger.LogInformation("Iniciando orquestación de archivo con parser: {ParserFormat}", parser.FormatId);

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

        logger.LogInformation(
            "Finalizada la orquestación. Total Procesados: {TotalProcessed}, Exitosos: {TotalSuccessful}, Fallidos: {TotalFailed}", 
            response.Summary.TotalProcessed, 
            response.Summary.TotalSuccessful, 
            response.Summary.TotalFailed);

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
        var limit = LogisticaConstants.MAX_NORMALIZATION_RECORDS;
        var processedIds = new HashSet<string>(limit); 
        var errors = new List<DeliveryError>(limit);
        var normalizedData = new List<DeliveryOrder>(limit); 

        int totalProcessed = 0;
        int totalSuccessful = 0;
        int totalFailed = 0;

        await foreach (var item in reader.ReadAllAsync(cancellationToken))
        {
            totalProcessed++;

            if (item.Error != null)
            {
                errors.Add(item.Error);
                totalFailed++;
                continue;
            }

            var order = item.Order!;

            if (!processedIds.Add(order.OrderId))
            {
                errors.Add(new DeliveryError(totalProcessed, order.OrderId, "DUPLICATE_ORDER", "La orden ya existe en este archivo."));
                totalFailed++;
                continue;
            }

            var validationErrors = OrderValidator.Validate(order, totalProcessed).ToList();
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
            logger.LogInformation("Canal finalizado. Delegando persistencia atómica de {ValidCount} registros consolidados.", normalizedData.Count);
            await repository.BulkInsertAsync(normalizedData, cancellationToken);
        }

        var summary = new ProcessingSummary(totalProcessed, totalSuccessful, totalFailed);
        return new NormalizationResponse(summary, errors, normalizedData);
    }
}
