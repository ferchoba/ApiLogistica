using Logistica.Application.Dtos;
using Logistica.Domain.Entities;
using Logistica.Domain.Interfaces;
using Logistica.Infrastructure.Parsers.Serialization;
using Logistica.Infrastructure.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;

namespace Logistica.Infrastructure.Parsers;

public class JsonParser : IDeliveryParser
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        TypeInfoResolver = JsonContext.Default,
        PropertyNameCaseInsensitive = true
    };

    public string FormatId => "JSON";

    public IAsyncEnumerable<(DeliveryOrder? Order, DeliveryError? Error)> ParseAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var items = JsonSerializer.DeserializeAsyncEnumerable<JsonDto>(stream, _serializerOptions, cancellationToken);
            return ParseItemsAsync(items, cancellationToken);
        }
        catch (Exception ex)
        {
            return GetErrorEnumerable(ex.Message);
        }
    }

    private static async IAsyncEnumerable<(DeliveryOrder? Order, DeliveryError? Error)> GetErrorEnumerable(
        string errorMessage)
    {
        await Task.CompletedTask;
        yield return (null, new DeliveryError(0, "N/A", "JSON_ROOT_ERROR", string.Format(InfraMessages.Parser_InvalidJsonStructure, errorMessage)));
    }

    private static async IAsyncEnumerable<(DeliveryOrder? Order, DeliveryError? Error)> ParseItemsAsync(
        IAsyncEnumerable<JsonDto?> items,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        int rowNumber = 0;
        await foreach (var dto in items.WithCancellation(cancellationToken))
        {
            rowNumber++;
            if (dto == null)
            {
                yield return (null, new DeliveryError(rowNumber, "N/A", "NULL_RECORD", InfraMessages.Parser_NullJsonRecord));
                continue;
            }

            var order = new DeliveryOrder(
                OrderId: dto.ShipmentNumber ?? string.Empty,
                Customer: dto.Recipient ?? string.Empty,
                Address: dto.Destination ?? string.Empty,
                DeliveryDate: dto.ScheduledDate,
                Weight: dto.PackageWeight
            );

            yield return (order, null);
        }
    }
}
