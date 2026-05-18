using Logistica.Domain.Interfaces;
using Logistica.Application.Services;
using Logistica.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Logistica.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class DeliveriesController(
        INormalizationOrchestrator orchestrator,
        IEnumerable<IDeliveryParser> parsers) : ControllerBase
    {

        [HttpPost("upload/{format}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NormalizationResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadFile(string format, IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { Error = "El archivo es inválido o está vacío." });
            
            var selectedParser = parsers.FirstOrDefault(
                p => p.FormatId.Equals(format, StringComparison.OrdinalIgnoreCase));
            
            if (selectedParser == null)
                return BadRequest(new { Error = $"Formato '{format}' no soportado." });
            
            using var stream = file.OpenReadStream();
            
            // El orquestador retorna el DTO con las colecciones optimizadas y abstraídas
            var response = await orchestrator.ProcessFileAsync(
                stream, selectedParser, cancellationToken);
            
            // System.Text.Json serializa eficientemente el IEnumerable de manera subyacente
            return Ok(response);
        }
    }
}