using Microsoft.AspNetCore.Authorization;

namespace Dinners.Application.Authorization.Menus.Review.Update;

public sealed class UserCanOnlyUpdateTheirReviewsRequirement : IAuthorizationRequirement
{
}
