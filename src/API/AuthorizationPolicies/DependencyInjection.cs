using Dinners.Application.Authorization.Menus.DeleteOrUpdate;
using Dinners.Application.Authorization.Menus.Publish;
using Dinners.Application.Authorization.Menus.Review.Update;
using Dinners.Application.Authorization.Reservations.Access;
using Dinners.Application.Authorization.Reservations.Get;
using Dinners.Application.Authorization.Restaurants.DeleteRestaurant;
using Dinners.Application.Authorization.Restaurants.ModifyProperties;
using Dinners.Application.Authorization.Restaurants.RateRestaurant;
using Dinners.Application.Authorization.Restaurants.RateRestaurant.Clients;
using Microsoft.AspNetCore.Authorization;
using Dinners.Application.Authorization;

namespace API.AuthorizationPolicies;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policy.CanUserDeleteOrUpdateMenu, x =>
            {
                x.Requirements.Add(new CanUserUpdateOrDeleteMenuRequirement());
            });

            options.AddPolicy(Policy.CanUserPublishMenu, x =>
            {
                x.Requirements.Add(new CanUserPublishAMenuRequirement());
            });

            options.AddPolicy(Policy.CanAccessToReservation, x =>
            {
                x.Requirements.Add(new OnlyReservationClientAndRestaurantAdministratorCanGetTheReservationRequirement());
            });

            options.AddPolicy(Policy.CanGetReservation, x =>
            {
                x.Requirements.Add(new OnlyReservationClientCanAndRestaurantAdministratorsCanAccessToTheReservationRequirement());
            });

            options.AddPolicy(Policy.CanModifyRestaurantProperties, x =>
            {
                x.Requirements.Add(new UserMustBeAnAdministratorToModifyRestaurantPropertiesRequirement());
            });

            options.AddPolicy(Policy.UserCanRateRestaurant, x =>
            {
                x.Requirements.Add(new UserCannotBeARestaurantAdministratorToRateItsRestaurantRequirement());
            });

            options.AddPolicy(Policy.CanDeleteOrUpdateRate, x =>
            {
                x.Requirements.Add(new UserCanOnlyDeleteOrUpdateItsOwnRatesRequirement());
            });

            options.AddPolicy(Policy.CanRateRestaurantWhenHasVisitedIt, x =>
            {
                x.Requirements.Add(new UserMustHaveVisitedTheRestaurantToRateItRequirement());
            });

            options.AddPolicy(Policy.UserCanOnlyUpdateTheirReviews, x =>
            {
                x.Requirements.Add(new UserCanOnlyUpdateTheirReviewsRequirement());
            });
        });

        services.AddScoped<IAuthorizationHandler, CanUserUpdateOrDeleteMenuRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, CanUserPublishAMenuRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, OnlyReservationClientAccessTheReservationRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, OnlyReservationClientCanAndRestaurantAdministratorsCanAccessToTheReservationRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, UserMustBeAnAdministratorToModifyRestaurantPropertiesRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, UserCannotBeARestaurantAdministratorToRateItsRestaurantRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, UserCanOnlyDeleteOrUpdateItsOwnRatesRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, UserMustHaveVisitedTheRestaurantToRateItRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, UserCanOnlyUpdateTheirReviewsRequirementHandler>();

        return services;
    }
}
