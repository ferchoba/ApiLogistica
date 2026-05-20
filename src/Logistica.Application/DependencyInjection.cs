using Microsoft.Extensions.DependencyInjection;
using Logistica.Application.Interfaces;
using Logistica.Application.Services;

namespace Logistica.Application;


public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<INormalizationOrchestrator, NormalizationOrchestratorService>();

        return services;
    }
}
