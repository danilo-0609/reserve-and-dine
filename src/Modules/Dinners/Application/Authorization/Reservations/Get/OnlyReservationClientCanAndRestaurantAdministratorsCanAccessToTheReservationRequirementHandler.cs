using Dinners.Domain.Restaurants;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Dinners.Application.Authorization.Reservations.Get;

public class OnlyReservationClientCanAndRestaurantAdministratorsCanAccessToTheReservationRequirementHandler : AuthorizationHandler<OnlyReservationClientCanAndRestaurantAdministratorsCanAccessToTheReservationRequirement, Tuple<RestaurantId, Guid>>
{
    private readonly IRestaurantRepository _restaurantRepository;

    public OnlyReservationClientCanAndRestaurantAdministratorsCanAccessToTheReservationRequirementHandler(IRestaurantRepository restaurantRepository)
    {
        _restaurantRepository = restaurantRepository;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        OnlyReservationClientCanAndRestaurantAdministratorsCanAccessToTheReservationRequirement requirement, 
        Tuple<RestaurantId, Guid> resources)
    {
        string? userIdValue = context.User.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value.ToString();

        if (userIdValue is null)
        {
            return;
        }

        Match match = Regex.Match(userIdValue, @"\b([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})\b");

        Guid userId = Guid.Parse(match.Value);

        var restaurant = await _restaurantRepository
            .GetRestaurantById(resources.Item1);

        if (restaurant is null)
        {
            return;
        }

        if (resources.Item2 == userId || restaurant
            .RestaurantAdministrations
            .Any(r => r.AdministratorId == userId))
        {
            context.Succeed(requirement);
        }

        return;
    }
}
