using Microsoft.AspNetCore.Authorization;

namespace API.Modules.Users.Policies.Dinners.Menus;

public sealed class CanUpdateOrDeleteMenuRequirement : IAuthorizationRequirement
{

    public Guid MenuId { get; set; }

    public CanUpdateOrDeleteMenuRequirement(Guid menuId)
    {
        MenuId = menuId;
    }
}
