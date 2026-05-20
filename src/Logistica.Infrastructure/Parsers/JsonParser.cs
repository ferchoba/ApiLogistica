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

    public async IAsyncEnumerable<(DeliveryOrder? Order, DeliveryError? Error)> ParseAsync(
        Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int rowNumber = 0;
        IAsyncEnumerable<JsonDto?>? items = null;

        bool initError = false;
        string initErrorMessage = string.Empty;

        try
        {
            items = JsonSerializer.DeserializeAsyncEnumerable<JsonDto>(stream, _serializerOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            initError = true;
            initErrorMessage = ex.Message;
        }

        if (initError)
        {
            yield return (null, new DeliveryError(0, "N/A", "JSON_ROOT_ERROR", string.Format(InfraMessages.Parser_InvalidJsonStructure, initErrorMessage)));
            yield break;
        }

        if (items != null)
        {
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
}
