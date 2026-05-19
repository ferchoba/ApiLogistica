using Logistica.Domain.Entities;
using System.Collections.Generic;

namespace Logistica.Domain.Interfaces;

public interface IDeliveryRepository
{
    Task BulkInsertAsync(IReadOnlyList<DeliveryOrder> orders, CancellationToken cancellationToken = default);
}
