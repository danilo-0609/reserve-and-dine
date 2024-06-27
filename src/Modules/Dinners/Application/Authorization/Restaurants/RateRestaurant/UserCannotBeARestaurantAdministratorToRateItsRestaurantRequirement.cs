using Microsoft.AspNetCore.Authorization;

namespace Dinners.Application.Authorization.Restaurants.RateRestaurant;

public class UserCannotBeARestaurantAdministratorToRateItsRestaurantRequirement : IAuthorizationRequirement
{
}
