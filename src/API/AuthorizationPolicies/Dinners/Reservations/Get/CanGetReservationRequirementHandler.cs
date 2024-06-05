using Dinners.Domain.Reservations;
using Dinners.Domain.Restaurants;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.RegularExpressions;

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
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value.ToString();

        if (userIdValue is null)
        {
            return;
        }

        Match match = Regex.Match(userIdValue, @"\b([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})\b");

        Guid userId = Guid.Parse(match.Value);

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
