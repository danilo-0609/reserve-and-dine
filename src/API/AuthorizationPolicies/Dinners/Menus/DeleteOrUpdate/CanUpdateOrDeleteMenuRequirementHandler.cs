using Dinners.Domain.Menus;
using Dinners.Domain.Restaurants;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace API.AuthorizationPolicies.Dinners.Menus.DeleteOrUpdate;

public sealed class CanUpdateOrDeleteMenuRequirementHandler : AuthorizationHandler<CanUpdateOrDeleteMenuRequirement, Guid>
{
    private readonly IMenuRepository _menuRepository;
    private readonly IRestaurantRepository _restaurantRepository;

    public CanUpdateOrDeleteMenuRequirementHandler(IRestaurantRepository restaurantRepository, IMenuRepository menuRepository)
    {
        _restaurantRepository = restaurantRepository;
        _menuRepository = menuRepository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CanUpdateOrDeleteMenuRequirement requirement, Guid menuId)
    {
        string? userIdValue = context.User.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userIdValue is null)
        {
            await Task.FromResult(0);
            return;
        }

        Guid userId = Guid.Parse(userIdValue);

        var menu = await _menuRepository.GetByIdAsync(MenuId.Create(menuId), CancellationToken.None);

        if (menu is null)
        {
            await Task.FromResult(0);
            return;
        }

        var restaurant = await _restaurantRepository.GetRestaurantById(menu!.RestaurantId);

        if (restaurant is null)
        {
            await Task.FromResult(0);
            return;
        }

        if (restaurant.RestaurantAdministrations.Any(r => r.AdministratorId == userId))
        {
            context.Succeed(requirement);
        }

        await Task.FromResult(0);
        return;
    }
}
