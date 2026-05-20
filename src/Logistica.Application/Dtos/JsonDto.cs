using System;
using System.Text.Json.Serialization;

namespace Logistica.Application.Dtos;

public record JsonDto(
    [property: JsonPropertyName("shipmentNumber")] string ShipmentNumber,
    [property: JsonPropertyName("recipient")]     string Recipient,
    [property: JsonPropertyName("destination")]   string Destination,
    [property: JsonPropertyName("scheduledDate")] DateTime ScheduledDate,
    [property: JsonPropertyName("packageWeight")] decimal PackageWeight
);
