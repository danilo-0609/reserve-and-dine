using Dinners.Domain.Restaurants;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace API.AuthorizationPolicies.Dinners.Menus.Publish;

public sealed class CanPublishAMenuRequirementHandler : AuthorizationHandler<CanPublishAMenuRequirement, Guid>
{
    private readonly IRestaurantRepository _restaurantRepository;

    public CanPublishAMenuRequirementHandler(IRestaurantRepository restaurantRepository)
    {
        _restaurantRepository = restaurantRepository;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanPublishAMenuRequirement requirement,
        Guid restaurantId)
    {
        string? userIdValue = context.User.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userIdValue is null)
        {
            return;
        }

        Guid userId = Guid.Parse(userIdValue);

       
        var restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(restaurantId));

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
