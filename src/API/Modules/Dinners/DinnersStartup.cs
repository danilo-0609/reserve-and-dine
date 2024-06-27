using Dinners.Application;
using Dinners.Infrastructure;

namespace API.Modules.Dinners;

public static class DinnersStartup
{
    public static IServiceCollection AddDinners(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration);

        return services;
    }
}
