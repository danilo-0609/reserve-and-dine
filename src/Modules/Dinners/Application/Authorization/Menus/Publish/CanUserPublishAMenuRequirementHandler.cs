using Domain.Restaurants;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Dinners.Application.Authorization.Menus.Publish;

public sealed class CanUserPublishAMenuRequirementHandler : AuthorizationHandler<CanUserPublishAMenuRequirement, Restaurant>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanUserPublishAMenuRequirement requirement,
        Restaurant restaurant)
    {
        string? userIdValue = context.User.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userIdValue is null)
        {
            return Task.FromResult(0);
        }

        Match match = Regex.Match(userIdValue, @"\b([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})\b");

        Guid userId = Guid.Parse(match.Value);

        if (restaurant.RestaurantAdministrations.Any(r => r.AdministratorId == userId))
        {
            context.Succeed(requirement);
            return Task.FromResult(1);
        }

        context.Fail();
        return Task.FromResult(0);
    }
}
