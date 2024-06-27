using Dinners.Domain.Menus.MenuReviews;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Dinners.Application.Authorization.Menus.Review.Update;

public sealed class UserCanOnlyUpdateTheirReviewsRequirementHandler : AuthorizationHandler<UserCanOnlyUpdateTheirReviewsRequirement, MenuReview>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserCanOnlyUpdateTheirReviewsRequirement requirement, MenuReview menuReview)
    {
        string? userIdValue = context.User.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userIdValue is null)
        {
            return Task.FromResult(0);
        }

        Match match = Regex.Match(userIdValue, @"\b([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})\b");

        Guid userId = Guid.Parse(match.Value);

        if (menuReview.ClientId != userId)
        {
            context.Fail();
            return Task.FromResult(0);
        }

        context.Succeed(requirement);
        return Task.FromResult(0);
    }
}
