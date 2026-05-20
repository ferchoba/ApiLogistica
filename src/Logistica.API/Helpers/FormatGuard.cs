using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Logistica.API.Resources;

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
        {
            return ApiMessages.FormatGuard_EmptyFile;
        }

        if (!AllowedFormats.TryGetValue(formatId, out var allowed))
        {
            return string.Format(ApiMessages.FormatGuard_UnsupportedFormat, formatId);
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowed.Extensions.Contains(extension))
        {
            return string.Format(ApiMessages.FormatGuard_ExtensionMismatch,file.FileName,extension,formatId.ToUpperInvariant(),string.Join(" o ", allowed.Extensions));
        }

        var contentType = file.ContentType?.ToLowerInvariant();
        if (!string.IsNullOrEmpty(contentType) && !allowed.MimeTypes.Contains(contentType))
        {
            return string.Format(ApiMessages.FormatGuard_MimeTypeMismatch,contentType,formatId.ToUpperInvariant());
        }

        return null;
    }
}
