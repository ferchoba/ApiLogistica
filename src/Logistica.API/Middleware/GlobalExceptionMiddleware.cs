using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Logistica.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Se ha producido una excepción no controlada en el sistema.");
            await HandleExceptionAsync(context);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            statusCode = context.Response.StatusCode,
            errorCode = "INTERNAL_SERVER_ERROR",
            message = "Ha ocurrido un error inesperado procesando la solicitud. Por favor, contacte a soporte."
        };

        var jsonResponse = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(jsonResponse);
    }
}
