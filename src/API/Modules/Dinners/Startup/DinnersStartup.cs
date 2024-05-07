using Dinners.Application;
using Dinners.Infrastructure;

namespace API.Modules.Dinners.Startup;

public static class DinnersStartup
{
    public static IServiceCollection AddDinners(this IServiceCollection services, string redisConnectionString, string databaseConnectionString, string dockerDatabaseConnectionString)
    {
        services.AddApplication();
        services.AddInfrastructure(redisConnectionString, databaseConnectionString, dockerDatabaseConnectionString);

        return services;
    }
}
