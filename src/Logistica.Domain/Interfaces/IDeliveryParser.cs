using Logistica.Domain.Entities;

namespace Logistica.Domain.Interfaces;

public interface IDeliveryParser
{
    string FormatId { get; }


    IAsyncEnumerable<(DeliveryOrder? Order, DeliveryError? Error)> ParseAsync(Stream stream, CancellationToken cancellationToken = default);
}
