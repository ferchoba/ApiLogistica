using Logistica.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logistica.Infrastructure.Parsers;

public class DeliveryParserFactory(IEnumerable<IDeliveryParser> parsers) : IDeliveryParserFactory
{
    public IDeliveryParser? GetParser(string formatId)
    {
        return parsers.FirstOrDefault(p => p.FormatId.Equals(formatId, StringComparison.OrdinalIgnoreCase));
    }
}
