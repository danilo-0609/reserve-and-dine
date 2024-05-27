using Microsoft.AspNetCore.Authorization;

namespace API.AuthorizationPolicies.Dinners.Reservations.Access;

public sealed class CanAccessToReservationRequirement : IAuthorizationRequirement
{
}
