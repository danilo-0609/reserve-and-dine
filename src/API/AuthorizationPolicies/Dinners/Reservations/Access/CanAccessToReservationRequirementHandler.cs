using Dinners.Domain.Reservations;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace API.AuthorizationPolicies.Dinners.Reservations.Access;

public class CanAccessToReservationRequirementHandler : AuthorizationHandler<CanAccessToReservationRequirement, Guid>
{
    private readonly IReservationRepository _reservationRepository;

    public CanAccessToReservationRequirementHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CanAccessToReservationRequirement requirement, Guid resource)
    {
        string? userIdValue = context.User.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)
                    ?.Value;

        if (userIdValue is null)
        {
            return;
        }

        Guid userId = Guid.Parse(userIdValue);

        var reservation = await _reservationRepository.GetByIdAsync(ReservationId.Create(resource), CancellationToken.None);

        if (reservation!.ReservationAttendees.ClientId == userId)
        {
            context.Succeed(requirement);
        }

        return;
    }
}
