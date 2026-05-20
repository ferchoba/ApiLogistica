namespace Logistica.Domain.Interfaces;

public interface IDeliveryParserFactory
{
    IDeliveryParser? GetParser(string formatId);
}
