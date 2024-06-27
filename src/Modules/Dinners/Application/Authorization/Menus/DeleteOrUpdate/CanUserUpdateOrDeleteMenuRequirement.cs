using Microsoft.AspNetCore.Authorization;

namespace Dinners.Application.Authorization.Menus.DeleteOrUpdate;

public sealed class CanUserUpdateOrDeleteMenuRequirement :  IAuthorizationRequirement
{
}
