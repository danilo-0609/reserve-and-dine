using Microsoft.AspNetCore.Authorization;

namespace Dinners.Application.Authorization.Menus.Review;

public class UserMustHaveConsumedTheMenuToReviewRequirement : IAuthorizationRequirement
{
}
