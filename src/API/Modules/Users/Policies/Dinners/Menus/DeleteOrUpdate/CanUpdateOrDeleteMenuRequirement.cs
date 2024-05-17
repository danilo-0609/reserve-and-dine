using Microsoft.AspNetCore.Authorization;

namespace API.Modules.Users.Policies.Dinners.Menus;

public sealed class CanUpdateOrDeleteMenuRequirement : IAuthorizationRequirement
{
}
