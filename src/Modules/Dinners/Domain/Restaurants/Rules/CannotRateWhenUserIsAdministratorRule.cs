using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Restaurants.Errors;
using Dinners.Domain.Restaurants.RestaurantUsers;
using ErrorOr;

namespace Dinners.Domain.Restaurants.Rules;

internal sealed class CannotRateWhenUserIsAdministratorRule : IBusinessRule
{
    private readonly Guid _userId;
    private readonly List<RestaurantAdministration> _restaurantAdministrations;

    public CannotRateWhenUserIsAdministratorRule(Guid userId, List<RestaurantAdministration> restaurantAdministrations)
    {
        _userId = userId;
        _restaurantAdministrations = restaurantAdministrations;
    }

    public Error Error => RestaurantErrorCodes.RateWhenUserIsAdministrator;

    public bool IsBroken() => _restaurantAdministrations.Any(r => r.AdministratorId == _userId);

    public static string Message => "Cannot rate when user is a restaurant administrator";
}
