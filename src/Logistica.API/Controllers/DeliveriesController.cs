using Logistica.Domain.Interfaces;
using Logistica.Application.Services;
using Logistica.Application.Dtos;
using Logistica.API.Helpers;
using Logistica.API.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Logistica.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeliveriesController(
    INormalizationOrchestrator orchestrator,
    IDeliveryParserFactory parserFactory) : ControllerBase
{
    [HttpPost("upload/{format}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NormalizationResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadFile(string format, IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { Error = ApiMessages.Http_EmptyFile });
        
        var formatError = FormatGuard.ValidateFileMatchesFormat(format, file);
        if (formatError != null)
            return BadRequest(new { Error = formatError });
        
        var selectedParser = parserFactory.GetParser(format);
        if (selectedParser == null)
            return BadRequest(new { Error = string.Format(ApiMessages.Http_UnsupportedFormat, format) });
        
        using var stream = file.OpenReadStream();
        var response = await orchestrator.ProcessFileAsync(stream, selectedParser, cancellationToken);
        return Ok(response);
    }
}