using Microsoft.Extensions.DependencyInjection;
using Logistica.Domain.Interfaces;
using Logistica.Infrastructure.Parsers;
using Logistica.Infrastructure.Persistence.Sqlite;

namespace Logistica.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IDeliveryRepository, SqliteDeliveryRepository>();

        services.AddScoped<IDeliveryParser, CsvParser>();
        services.AddScoped<IDeliveryParser, JsonParser>();
        services.AddScoped<IDeliveryParser, TxtParser>();
        services.AddScoped<IDeliveryParser, XmlParser>();

        return services;
    }
}
