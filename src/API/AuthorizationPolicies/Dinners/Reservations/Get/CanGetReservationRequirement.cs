using Microsoft.AspNetCore.Authorization;

namespace API.AuthorizationPolicies.Dinners.Reservations.Get;

public sealed class CanGetReservationRequirement : IAuthorizationRequirement
{
}
