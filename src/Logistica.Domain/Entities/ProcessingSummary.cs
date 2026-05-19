namespace Logistica.Domain.Entities;

public record ProcessingSummary(
    int TotalProcessed,
    int TotalSuccessful,
    int TotalFailed
);
