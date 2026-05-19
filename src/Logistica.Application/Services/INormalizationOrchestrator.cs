using Logistica.Domain.Interfaces;
using Logistica.Application.Dtos;

namespace Logistica.Application.Services;

public interface INormalizationOrchestrator
{
    Task<NormalizationResponse> ProcessFileAsync(Stream fileStream,IDeliveryParser parser,CancellationToken cancellationToken = default);
}
