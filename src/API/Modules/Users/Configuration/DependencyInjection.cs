using API.Modules.Users.Entities;
using API.Modules.Users.Policies.Dinners;
using API.Modules.Users.Policies.Dinners.Menus.Publish;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Modules.Users.Configuration;

public static class DependencyInjection 
{
    public static IServiceCollection AddUsers(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policy.CanPublishMenu, policy =>
            {
                policy.RequireRole(Role.RestaurantAdministrator.Value);
                policy.Requirements.Add(new CanPublishAMenuRequirement());
            });

        });

        services.AddTransient<IAuthorizationHandler, CanPublishAMenuRequirementHandler>();

        services.AddAuthentication()
            .AddCookie(IdentityConstants.ApplicationScheme)
            .AddBearerToken(IdentityConstants.BearerScheme);

        services.AddDbContext<UsersDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DockerSqlDatabase"), 
                r => r.EnableRetryOnFailure(4));
        });

        services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<UsersDbContext>()
            .AddApiEndpoints();

        return services;
    }
}
