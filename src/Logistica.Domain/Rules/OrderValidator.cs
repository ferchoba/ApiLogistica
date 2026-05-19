using Logistica.Domain.Entities;

namespace Logistica.Domain.Rules;

public static class OrderValidator
{

    public static IEnumerable<DeliveryError> Validate(DeliveryOrder order, int rowNumber)
    {

        if (string.IsNullOrWhiteSpace(order.OrderId))
        {
            yield return new DeliveryError(rowNumber, string.Empty, "MISSING_IDENTITY", "El identificador de la orden es obligatorio.");
        }


        if (string.IsNullOrWhiteSpace(order.Customer) || string.IsNullOrWhiteSpace(order.Address))
        {
            yield return new DeliveryError(rowNumber, order.OrderId, "INVALID_DESTINATION", "El destinatario y la dirección son obligatorios.");
        }


        if (order.Weight <= 0)
        {
            yield return new DeliveryError(rowNumber, order.OrderId, "INVALID_WEIGHT", "El peso debe ser mayor a 0.");
        }


        if (order.DeliveryDate.Date < DateTime.UtcNow.Date)
        {
            yield return new DeliveryError(rowNumber, order.OrderId, "INVALID_DATE", "La fecha de entrega no puede ser inferior a la fecha actual.");
        }
    }
}
