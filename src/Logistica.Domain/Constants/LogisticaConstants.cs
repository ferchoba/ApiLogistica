namespace Logistica.Domain.Constants;

/// <summary>
/// Provee una fuente única de verdad para los invariantes técnicos y de negocio del sistema.
/// Elimina el uso de "Magic Numbers" siguiendo los estándares de Clean Code.
/// </summary>
public static class LogisticaConstants
{
    /// <summary>
    /// Límite máximo de registros permitidos en una operación de normalización masiva.
    /// Definido en base a los requerimientos de capacidad (50,000 registros).
    /// </summary>
    public const int MAX_NORMALIZATION_RECORDS = 50_000;

    /// <summary>
    /// Capacidad del canal de comunicación (Producer-Consumer) para el procesamiento en streaming.
    /// </summary>
    public const int DEFAULT_CHANNEL_CAPACITY = 5_000;
}
