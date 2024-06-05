using Dinners.Domain.Menus;
using Dinners.Domain.Restaurants;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.RegularExpressions;

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
            return;
        }

        Match match = Regex.Match(userIdValue, @"\b([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})\b");

        Guid userId = Guid.Parse(match.Value);

        var menu = await _menuRepository.GetByIdAsync(MenuId.Create(menuId), CancellationToken.None);

        if (menu is null)
        {
            return;
        }

        var restaurant = await _restaurantRepository.GetRestaurantById(menu!.RestaurantId);

        if (restaurant is null)
        {
            return;
        }

        if (restaurant.RestaurantAdministrations.Any(r => r.AdministratorId == userId))
        {
            context.Succeed(requirement);
        }

        return;
    }
}
