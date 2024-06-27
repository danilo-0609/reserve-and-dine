using Microsoft.AspNetCore.Authorization;

namespace Dinners.Application.Authorization.Restaurants.DeleteRestaurant;

public class UserCanOnlyDeleteOrUpdateItsOwnRatesRequirement : IAuthorizationRequirement
{
}
