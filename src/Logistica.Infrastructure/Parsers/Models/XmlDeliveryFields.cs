namespace Logistica.Infrastructure.Parsers.Models;

internal sealed class XmlDeliveryFields
{
    public string OrderId { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string DateStr { get; set; } = string.Empty;
    public string WeightStr { get; set; } = string.Empty;
}
