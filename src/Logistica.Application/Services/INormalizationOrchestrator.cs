using Logistica.Domain.Interfaces;
using Logistica.Application.Dtos;

namespace Logistica.Application.Services
{
    /// <summary>
    /// Contrato arquitectónico para la orquestación y normalización de archivos de entregas.
    /// Permite el desacoplamiento de los adaptadores primarios según el principio DIP.
    /// </summary>
    public interface INormalizationOrchestrator
    {
        Task<NormalizationResponse> ProcessFileAsync(
            Stream fileStream,
            IDeliveryParser parser,
            CancellationToken cancellationToken = default);
    }
}
