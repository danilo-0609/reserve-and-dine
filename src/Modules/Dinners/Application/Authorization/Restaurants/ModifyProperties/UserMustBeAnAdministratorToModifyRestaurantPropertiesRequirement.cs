using Microsoft.AspNetCore.Authorization;

namespace Dinners.Application.Authorization.Restaurants.ModifyProperties;

public class UserMustBeAnAdministratorToModifyRestaurantPropertiesRequirement : IAuthorizationRequirement
{
}
