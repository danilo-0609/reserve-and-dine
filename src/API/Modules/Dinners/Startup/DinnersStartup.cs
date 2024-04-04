using Dinners.Application;
using Dinners.Infrastructure;

namespace API.Modules.Dinners.Startup;

public static class DinnersStartup
{
    public static IServiceCollection AddDinners(this IServiceCollection services, string databaseConnectionString, string redisConnectionString)
    {
        services.AddApplication();
        services.AddInfrastructure(databaseConnectionString, redisConnectionString);

        return services;
    }
}
