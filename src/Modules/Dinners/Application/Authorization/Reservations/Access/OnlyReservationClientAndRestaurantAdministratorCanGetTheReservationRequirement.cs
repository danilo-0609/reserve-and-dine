using Microsoft.AspNetCore.Authorization;

namespace Dinners.Application.Authorization.Reservations.Access;

public class OnlyReservationClientAndRestaurantAdministratorCanGetTheReservationRequirement : IAuthorizationRequirement
{

}