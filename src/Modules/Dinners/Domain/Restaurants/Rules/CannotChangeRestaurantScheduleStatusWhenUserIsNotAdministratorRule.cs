using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Restaurants.Errors;
using Dinners.Domain.Restaurants.RestaurantUsers;
using ErrorOr;

namespace Dinners.Domain.Restaurants.Rules;

internal sealed class CannotChangeRestaurantScheduleStatusWhenUserIsNotAdministratorRule : IBusinessRule
{
    private readonly List<RestaurantAdministration> _restaurantAdministrations;
    private readonly Guid _userId;

    public CannotChangeRestaurantScheduleStatusWhenUserIsNotAdministratorRule(List<RestaurantAdministration> restaurantAdministrations, Guid userId)
    {
        _restaurantAdministrations = restaurantAdministrations;
        _userId = userId;
    }

    public Error Error => RestaurantErrorCodes.CannotChangeRestaurantScheduleStatus;

    public bool IsBroken() => !_restaurantAdministrations.Any(r => r.AdministratorId == _userId);

    public static string Message => "Cannot change restaurant schedule status when user is not administrator";
}
