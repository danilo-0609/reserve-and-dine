using Dinners.Domain.Menus;
using Dinners.Domain.Restaurants;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Dinners.Application.Authorization.Menus.DeleteOrUpdate;

public sealed class CanUserUpdateOrDeleteMenuRequirementHandler : AuthorizationHandler<CanUserUpdateOrDeleteMenuRequirement, Menu>
{
    private readonly IRestaurantRepository _restaurantRepository;

    public CanUserUpdateOrDeleteMenuRequirementHandler(IRestaurantRepository restaurantRepository)
    {
        _restaurantRepository = restaurantRepository;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        CanUserUpdateOrDeleteMenuRequirement requirement, 
        Menu menu)
    {
        string? userIdValue = context.User.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userIdValue is null)
        {
            return;
        }

        Match match = Regex.Match(userIdValue, @"\b([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})\b");

        Guid userId = Guid.Parse(match.Value);

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
