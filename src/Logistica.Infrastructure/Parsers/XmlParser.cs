using Logistica.Domain.Entities;
using Logistica.Domain.Interfaces;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Logistica.Infrastructure.Parsers
{
    public class XmlParser : IDeliveryParser
    {
        public string FormatId => "XML";

        public async IAsyncEnumerable<(DeliveryOrder? Order, DeliveryError? Error)> ParseAsync(
            Stream stream,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            
            var settings = new XmlReaderSettings { Async = true, IgnoreWhitespace = true };
            using var reader = XmlReader.Create(stream, settings);

            int rowNumber = 0;

            while (await reader.ReadAsync())
            {
                cancellationToken.ThrowIfCancellationRequested();

                
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Delivery")
                {
                    rowNumber++;
                    string orderId = string.Empty;
                    string customer = string.Empty;
                    string address = string.Empty;
                    string dateStr = string.Empty;
                    string weightStr = string.Empty;

                    DeliveryOrder? order = null;
                    DeliveryError? error = null;
                    bool missingCode = false;

                    try
                    {
                        
                        using var subReader = reader.ReadSubtree();
                        while (await subReader.ReadAsync())
                        {
                            if (subReader.NodeType == XmlNodeType.Element)
                            {
                                switch (subReader.Name)
                                {
                                    case "Code":
                                        orderId = await subReader.ReadElementContentAsStringAsync();
                                        break;
                                    case "ClientName":
                                        customer = await subReader.ReadElementContentAsStringAsync();
                                        break;
                                    case "Location":
                                        address = await subReader.ReadElementContentAsStringAsync();
                                        break;
                                    case "Date":
                                        dateStr = await subReader.ReadElementContentAsStringAsync();
                                        break;
                                    case "Kg":
                                        weightStr = await subReader.ReadElementContentAsStringAsync();
                                        break;
                                }
                            }
                        }

                        if (string.IsNullOrWhiteSpace(orderId))
                        {
                            missingCode = true;
                        }
                        else
                        {
                            order = new DeliveryOrder(
                                OrderId: orderId.Trim(),
                                Customer: customer.Trim(),
                                Address: address.Trim(),
                                DeliveryDate: DateTime.ParseExact(dateStr.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture),
                                Weight: decimal.Parse(weightStr.Trim(), CultureInfo.InvariantCulture)
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        error = new DeliveryError(rowNumber, orderId, "PARSE_ERROR", $"Error procesando nodo XML: {ex.Message}");
                    }

                    if (missingCode)
                    {
                        yield return (null, new DeliveryError(rowNumber, "N/A", "INVALID_FORMAT", "Falta el nodo obligatorio <Code>."));
                    }
                    else if (error != null)
                    {
                        yield return (null, error);
                    }
                    else if (order != null)
                    {
                        yield return (order, null);
                    }
                }
            }
        }
    }
}
