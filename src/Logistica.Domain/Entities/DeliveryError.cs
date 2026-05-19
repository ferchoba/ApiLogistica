namespace Logistica.Domain.Entities;

public record DeliveryError(
    int RowNumber,
    string OrderId,
    string ErrorCode,
    string Message
);
