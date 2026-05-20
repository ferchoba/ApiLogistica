using Logistica.Domain.Entities;
using Logistica.Domain.Interfaces;
using Logistica.Infrastructure.Parsers.Models;
using Logistica.Infrastructure.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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
        var fields = new XmlDeliveryFields();

        try
        {
            await ReadDeliveryFieldsAsync(reader, fields);
        }
        catch (Exception ex)
        {
            return (null, new DeliveryError(rowNumber, fields.OrderId, "PARSE_ERROR",
                string.Format(InfraMessages.Parser_XmlProcessError, ex.Message)));
        }

        if (string.IsNullOrWhiteSpace(fields.OrderId))
        {
            return (null, new DeliveryError(rowNumber, "N/A", "INVALID_FORMAT",
                InfraMessages.Parser_MissingRequiredXmlNode));
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

    private static async Task ReadDeliveryFieldsAsync(XmlReader reader, XmlDeliveryFields fields)
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
}
