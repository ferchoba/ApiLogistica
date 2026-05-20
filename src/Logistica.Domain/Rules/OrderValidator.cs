using Logistica.Domain.Entities;
using Logistica.Domain.Resources;

namespace Logistica.Domain.Rules;

public static class OrderValidator
{
    public static IEnumerable<DeliveryError> Validate(DeliveryOrder order, int rowNumber)
    {
        if (string.IsNullOrWhiteSpace(order.OrderId))
        {
            yield return new DeliveryError(rowNumber, string.Empty, "MISSING_IDENTITY", DomainMessages.Validation_MissingIdentity);
        }

        if (string.IsNullOrWhiteSpace(order.Customer) || string.IsNullOrWhiteSpace(order.Address))
        {
            yield return new DeliveryError(rowNumber, order.OrderId, "INVALID_DESTINATION", DomainMessages.Validation_InvalidDestination);
        }

        if (order.Weight <= 0)
        {
            yield return new DeliveryError(rowNumber, order.OrderId, "INVALID_WEIGHT", DomainMessages.Validation_InvalidWeight);
        }

        if (order.DeliveryDate.Date < DateTime.UtcNow.Date)
        {
            yield return new DeliveryError(rowNumber, order.OrderId, "INVALID_DATE", DomainMessages.Validation_InvalidDate);
        }
    }
}
