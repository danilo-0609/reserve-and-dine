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
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policy.CanDeleteOrUpdateMenu, x =>
            {
                x.Requirements.Add(new CanUpdateOrDeleteMenuRequirement());
            });

            options.AddPolicy(Policy.CanPublishMenu, x =>
            {
                x.Requirements.Add(new CanPublishAMenuRequirement());
            });

            options.AddPolicy(Policy.CanAccessToReservation, x =>
            {
                x.Requirements.Add(new CanAccessToReservationRequirement());
            });

            options.AddPolicy(Policy.CanGetReservation, x =>
            {
                x.Requirements.Add(new CanGetReservationRequirement());
            });
        });

        services.AddScoped<IAuthorizationHandler, CanUpdateOrDeleteMenuRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, CanPublishAMenuRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, CanAccessToReservationRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, CanGetReservationRequirementHandler>();

        return services;
    }
}
