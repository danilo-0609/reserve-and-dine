using Microsoft.AspNetCore.Authorization;

namespace Dinners.Application.Authorization.Reservations.Get;

public sealed class OnlyReservationClientCanAndRestaurantAdministratorsCanAccessToTheReservationRequirement : IAuthorizationRequirement
{
}
