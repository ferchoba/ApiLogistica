using Logistica.Domain.Entities;
using Logistica.Domain.Interfaces;
using Logistica.Infrastructure.Resources;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Logistica.Infrastructure.Parsers;

public abstract class DelimitedTextParser : IDeliveryParser
{
    protected readonly ILogger _logger;

    protected DelimitedTextParser(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public abstract string FormatId { get; }
    protected abstract char Delimiter { get; }
    protected abstract string DateFormat { get; }
    protected abstract bool HasHeader { get; }

    public async IAsyncEnumerable<(DeliveryOrder? Order, DeliveryError? Error)> ParseAsync(
        Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(InfraMessages.Parser_StartRead, FormatId);
        
        using var reader = new StreamReader(stream);
        int rowNumber = 0;

        if (HasHeader)
        {
            string? header = await reader.ReadLineAsync(cancellationToken);
            if (header == null) yield break; 
            rowNumber++;
        }

        while ((await reader.ReadLineAsync(cancellationToken)) is { } line)
        {
            rowNumber++;
            if (string.IsNullOrWhiteSpace(line)) continue;

            var columns = line.Split(Delimiter);
            yield return BuildOrderResult(columns, rowNumber);
        }

        _logger.LogInformation(InfraMessages.Parser_EndRead, FormatId, rowNumber);
    }

    protected (DeliveryOrder? Order, DeliveryError? Error) BuildOrderResult(string[] columns, int rowNumber)
    {
        if (columns.Length < 5)
        {
            return (null, new DeliveryError(rowNumber, "N/A", "INVALID_FORMAT", string.Format(InfraMessages.Parser_InsufficientColumns, FormatId)));
        }

        string orderId = columns[0].Trim();

        try
        {
            var order = new DeliveryOrder(
                OrderId: orderId,
                Customer: columns[1].Trim(),
                Address: columns[2].Trim(),
                DeliveryDate: DateTime.ParseExact(columns[3].Trim(), DateFormat, CultureInfo.InvariantCulture),
                Weight: decimal.Parse(columns[4].Trim(), CultureInfo.InvariantCulture)
            );
            return (order, null);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, InfraMessages.Parser_ErrorParsingLine, rowNumber, orderId);
            var error = new DeliveryError(rowNumber, orderId, "PARSE_ERROR", string.Format(InfraMessages.Parser_ConversionError, FormatId, ex.Message));
            return (null, error);
        }
    }
}
