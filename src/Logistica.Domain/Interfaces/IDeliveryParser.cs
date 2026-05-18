using Logistica.Domain.Entities;
using System.Runtime.CompilerServices;

namespace Logistica.Domain.Interfaces
{
    public interface IDeliveryParser
    {
        string FormatId { get; }


        IAsyncEnumerable<(DeliveryOrder? Order, DeliveryError? Error)> ParseAsync(
            Stream stream,
            [EnumeratorCancellation] CancellationToken cancellationToken = default);
    }
}
