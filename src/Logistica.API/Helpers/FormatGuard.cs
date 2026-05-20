using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Logistica.API.Helpers;

public static class FormatGuard
{
    private static readonly Dictionary<string, (string[] Extensions, string[] MimeTypes)> AllowedFormats = new(StringComparer.OrdinalIgnoreCase)
    {
        { "CSV", (new[] { ".csv" }, new[] { "text/csv", "text/plain", "application/csv", "application/vnd.ms-excel" }) },
        { "TXT", (new[] { ".txt" }, new[] { "text/plain" }) },
        { "JSON", (new[] { ".json" }, new[] { "application/json", "text/json" }) },
        { "XML", (new[] { ".xml" }, new[] { "application/xml", "text/xml" }) }
    };

    public static string? ValidateFileMatchesFormat(string formatId, IFormFile file)
    {
        if (file == null)
            return "El archivo no puede ser nulo o vacío.";

        if (!AllowedFormats.TryGetValue(formatId, out var allowed))
            return $"El formato de destino '{formatId}' no es soportado por el sistema.";

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowed.Extensions.Contains(extension))
        {
            return $"Inconsistencia de formato: El archivo '{file.FileName}' tiene extensión '{extension}', " +
                   $"la cual no coincide con el formato declarado '{formatId.ToUpperInvariant()}' (se esperaba: {string.Join(" o ", allowed.Extensions)}).";
        }

        var contentType = file.ContentType?.ToLowerInvariant();
        if (!string.IsNullOrEmpty(contentType) && !allowed.MimeTypes.Contains(contentType))
        {
            return $"Inconsistencia de seguridad de formato: El archivo declara un Content-Type '{contentType}', " +
                   $"el cual no es compatible con el formato '{formatId.ToUpperInvariant()}'.";
        }

        return null;
    }
}
