namespace Logistica.Domain.Entities;

public record DeliveryOrder(
    string OrderId,
    string Customer,
    string Address,
    DateTime DeliveryDate,
    decimal Weight
);
