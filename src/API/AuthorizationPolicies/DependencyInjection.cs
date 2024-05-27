using API.AuthorizationPolicies.Dinners.Menus.DeleteOrUpdate;
using API.AuthorizationPolicies.Dinners.Menus.Publish;
using API.AuthorizationPolicies.Dinners.Reservations.Access;
using API.AuthorizationPolicies.Dinners.Reservations.Get;
using Microsoft.AspNetCore.Authorization;

namespace API.AuthorizationPolicies;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddTransient<IAuthorizationRequirement, CanUpdateOrDeleteMenuRequirement>();
        services.AddTransient<IAuthorizationRequirement, CanPublishAMenuRequirement>();
        services.AddTransient<IAuthorizationRequirement, CanAccessToReservationRequirement>();
        services.AddTransient<IAuthorizationRequirement, CanGetReservationRequirement>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policy.CanDeleteOrUpdateMenu, x =>
            {
                x.AddRequirements(new CanUpdateOrDeleteMenuRequirement());
            });

            options.AddPolicy(Policy.CanPublishMenu, x =>
            {
                x.AddRequirements(new CanPublishAMenuRequirement());
            });

            options.AddPolicy(Policy.CanAccessToReservation, x =>
            {
                x.AddRequirements(new CanAccessToReservationRequirement());
            });

            options.AddPolicy(Policy.CanGetReservation, x =>
            {
                x.AddRequirements(new CanGetReservationRequirement());
            });
        });

        return services;
    }
}
