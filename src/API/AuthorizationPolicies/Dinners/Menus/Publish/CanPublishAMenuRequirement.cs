using Microsoft.AspNetCore.Authorization;

namespace API.AuthorizationPolicies.Dinners.Menus.Publish;

public sealed class CanPublishAMenuRequirement : IAuthorizationRequirement
{
}
