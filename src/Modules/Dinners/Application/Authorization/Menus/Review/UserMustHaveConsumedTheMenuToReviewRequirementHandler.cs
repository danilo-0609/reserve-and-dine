using Dinners.Domain.Menus;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Dinners.Application.Authorization.Menus.Review;

public sealed class UserMustHaveConsumedTheMenuToReviewRequirementHandler : AuthorizationHandler<UserMustHaveConsumedTheMenuToReviewRequirement, Menu>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserMustHaveConsumedTheMenuToReviewRequirement requirement, Menu menu)
    {
        string? userIdValue = context.User.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userIdValue is null)
        {
            return Task.FromResult(0);
        }

        Match match = Regex.Match(userIdValue, @"\b([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})\b");

        Guid userId = Guid.Parse(match.Value);

        if (menu.MenuConsumers.Any(r => r.ClientId == userId))
        {
            context.Succeed(requirement);
        }

        return Task.FromResult(0);
    }
}
