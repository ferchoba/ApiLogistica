using Logistica.Domain.Entities;
using Logistica.Domain.Resources;

namespace Logistica.Domain.Rules;

public static class OrderValidator
{
    private record ValidationRule(
        Func<DeliveryOrder, bool> IsInvalid,
        string ErrorCode,
        string Message,
        Func<DeliveryOrder, string> GetOrderId
    );

    private static readonly ValidationRule[] Rules = 
    [
        new(order => string.IsNullOrWhiteSpace(order.OrderId), "MISSING_IDENTITY", DomainMessages.Validation_MissingIdentity, _ => string.Empty),
        new(order => string.IsNullOrWhiteSpace(order.Customer) || string.IsNullOrWhiteSpace(order.Address), "INVALID_DESTINATION", DomainMessages.Validation_InvalidDestination, order => order.OrderId ?? string.Empty),
        new(order => order.Weight <= 0, "INVALID_WEIGHT", DomainMessages.Validation_InvalidWeight, order => order.OrderId ?? string.Empty),
        new(order => order.DeliveryDate.Date < DateTime.UtcNow.Date, "INVALID_DATE", DomainMessages.Validation_InvalidDate, order => order.OrderId ?? string.Empty)
    ];

    public static IEnumerable<DeliveryError> Validate(DeliveryOrder order, int rowNumber)
    {
        var failedRules = Rules.Where(rule => rule.IsInvalid(order));

        foreach (var rule in failedRules)
        {
            yield return new DeliveryError(
                rowNumber,
                rule.GetOrderId(order),
                rule.ErrorCode,
                rule.Message
            );
        }
    }
}
