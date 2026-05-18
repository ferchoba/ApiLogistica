using Logistica.Domain.Entities;

namespace Logistica.Application.Dtos;

public record NormalizationResponse(
    ProcessingSummary Summary,
    IEnumerable<DeliveryError> Errors,
    IEnumerable<DeliveryOrder> NormalizedData
);
