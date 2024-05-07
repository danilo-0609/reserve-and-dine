using Dinners.Domain.Restaurants;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace API.Modules.Users.Policies.Dinners.Menus.Publish;

public sealed class CanPublishAMenuRequirementHandler : AuthorizationHandler<CanPublishAMenuRequirement, Guid>
{
    private readonly IServiceProvider _serviceProvider;

    public CanPublishAMenuRequirementHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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
            await Task.FromResult(0);
            return;
        }

        Guid userId = Guid.Parse(userIdValue);

        using (var scope = _serviceProvider.CreateAsyncScope())
        {
            var restaurantRepository = scope.ServiceProvider.GetRequiredService<IRestaurantRepository>();

            var restaurant = await restaurantRepository.GetRestaurantById(RestaurantId.Create(restaurantId));

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
}
