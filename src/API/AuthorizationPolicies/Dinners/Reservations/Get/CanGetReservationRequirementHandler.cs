using Dinners.Domain.Reservations;
using Dinners.Domain.Restaurants;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace API.AuthorizationPolicies.Dinners.Reservations.Get;

public class CanGetReservationRequirementHandler : AuthorizationHandler<CanGetReservationRequirement, Guid>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRestaurantRepository _restaurantRepository;

    public CanGetReservationRequirementHandler(IReservationRepository reservationRepository, IRestaurantRepository restaurantRepository)
    {
        _reservationRepository = reservationRepository;
        _restaurantRepository = restaurantRepository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CanGetReservationRequirement requirement, Guid resource)
    {
        string? userIdValue = context.User.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userIdValue is null)
        {
            return;
        }

        Guid userId = Guid.Parse(userIdValue);

        var reservation = await _reservationRepository
            .GetByIdAsync(ReservationId.Create(resource), CancellationToken.None);

        if (reservation is null)
        {
            return;
        }

        var restaurant = await _restaurantRepository
            .GetRestaurantById(reservation!.RestaurantId);

        if (restaurant is null)
        {
            return;
        }

        if (reservation.ReservationAttendees.ClientId == userId ||
            restaurant.RestaurantAdministrations.Any(r => r.AdministratorId == userId))
        {
            context.Succeed(requirement);
        }

        return;
    }
}
