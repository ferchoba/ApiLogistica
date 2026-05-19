using Logistica.Domain.Entities;
using Logistica.Domain.Interfaces;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Logistica.Infrastructure.Parsers;

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

            if (!IsDeliveryElement(reader)) continue;

            rowNumber++;
            yield return await ParseDeliveryNodeAsync(reader, rowNumber);
        }
    }


    private static bool IsDeliveryElement(XmlReader reader)
        => reader.NodeType == XmlNodeType.Element && reader.Name == "Delivery";


    private static async Task<(DeliveryOrder? Order, DeliveryError? Error)> ParseDeliveryNodeAsync(
        XmlReader reader, int rowNumber)
    {
        var fields = new DeliveryFields();

        try
        {
            await ReadDeliveryFieldsAsync(reader, fields);
        }
        catch (Exception ex)
        {
            return (null, new DeliveryError(rowNumber, fields.OrderId, "PARSE_ERROR",
                $"Error procesando nodo XML: {ex.Message}"));
        }

        if (string.IsNullOrWhiteSpace(fields.OrderId))
        {
            return (null, new DeliveryError(rowNumber, "N/A", "INVALID_FORMAT",
                "Falta el nodo obligatorio <Code>."));
        }

        

        var order = new DeliveryOrder(
            OrderId: fields.OrderId.Trim(),
            Customer: fields.Customer.Trim(),
            Address: fields.Address.Trim(),
            DeliveryDate: DateTime.ParseExact(fields.DateStr.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture),
            Weight: decimal.Parse(fields.WeightStr.Trim(), CultureInfo.InvariantCulture)
        );

        return (order, null);
    }


    private static async Task ReadDeliveryFieldsAsync(XmlReader reader, DeliveryFields fields)
    {
        using var subReader = reader.ReadSubtree();
        while (await subReader.ReadAsync())
        {
            if (subReader.NodeType != XmlNodeType.Element) continue;

            switch (subReader.Name)
            {
                case "Code":       fields.OrderId = await subReader.ReadElementContentAsStringAsync(); break;
                case "ClientName": fields.Customer = await subReader.ReadElementContentAsStringAsync(); break;
                case "Location":   fields.Address = await subReader.ReadElementContentAsStringAsync(); break;
                case "Date":       fields.DateStr = await subReader.ReadElementContentAsStringAsync(); break;
                case "Kg":         fields.WeightStr = await subReader.ReadElementContentAsStringAsync(); break;
            }
        }
    }


    private sealed class DeliveryFields
    {
        public string OrderId { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string DateStr { get; set; } = string.Empty;
        public string WeightStr { get; set; } = string.Empty;
    }
}
