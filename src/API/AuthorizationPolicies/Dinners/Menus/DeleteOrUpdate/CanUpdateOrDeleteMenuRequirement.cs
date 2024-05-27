using Microsoft.AspNetCore.Authorization;

namespace API.AuthorizationPolicies.Dinners.Menus.DeleteOrUpdate;

public sealed class CanUpdateOrDeleteMenuRequirement :  IAuthorizationRequirement
{
}
