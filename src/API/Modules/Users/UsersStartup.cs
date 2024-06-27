using Users.Application;
using Users.Infrastructure;

namespace API.Modules.Users;

public static class UsersStartup
{
    public static IServiceCollection AddUsers(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration);

        return services;
    }
}
