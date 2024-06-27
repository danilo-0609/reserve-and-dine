using Microsoft.AspNetCore.Authorization;

namespace Dinners.Application.Authorization.Restaurants.RateRestaurant.Clients;

public class UserMustHaveVisitedTheRestaurantToRateItRequirement : IAuthorizationRequirement
{
}
