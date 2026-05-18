using Logistica.Domain.Entities;
using Logistica.Domain.Interfaces;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logistica.Infrastructure.Parsers
{
    public record OperadorBDto(
        [property: JsonPropertyName("shipmentNumber")] string ShipmentNumber,
        [property: JsonPropertyName("recipient")] string Recipient,
        [property: JsonPropertyName("destination")] string Destination,
        [property: JsonPropertyName("scheduledDate")] DateTime ScheduledDate,
        [property: JsonPropertyName("packageWeight")] decimal PackageWeight
    );
    
    [JsonSerializable(typeof(OperadorBDto))]
    [JsonSerializable(typeof(IAsyncEnumerable<OperadorBDto>))]
    public partial class OperadorBJsonContext : JsonSerializerContext { }

    public class JsonParser : IDeliveryParser
    {
        public string FormatId => "JSON";

        public async IAsyncEnumerable<(DeliveryOrder? Order, DeliveryError? Error)> ParseAsync(
            Stream stream,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            int rowNumber = 0;
            IAsyncEnumerable<OperadorBDto?>? items = null;

            bool initError = false;
            string initErrorMessage = string.Empty;

            try
            {
                
                var options = new JsonSerializerOptions
                {
                    TypeInfoResolver = OperadorBJsonContext.Default,
                    PropertyNameCaseInsensitive = true
                };

                items = JsonSerializer.DeserializeAsyncEnumerable<OperadorBDto>(stream, options, cancellationToken);
            }
            catch (Exception ex)
            {
                initError = true;
                initErrorMessage = ex.Message;
            }

            if (initError)
            {
                yield return (null, new DeliveryError(0, "N/A", "JSON_ROOT_ERROR", $"Estructura JSON inválida: {initErrorMessage}"));
                yield break;
            }

            if (items != null)
            {
                await foreach (var dto in items.WithCancellation(cancellationToken))
                {
                    rowNumber++;
                    if (dto == null)
                    {
                        yield return (null, new DeliveryError(rowNumber, "N/A", "NULL_RECORD", "El objeto JSON es nulo."));
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
}
