using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Restaurants.Errors;
using Dinners.Domain.Restaurants.RestaurantSchedules;
using ErrorOr;

namespace Dinners.Domain.Restaurants.Rules;

internal sealed class CannotCloseWhenRestaurantScheduleStatusIsClosedRule : IBusinessRule
{
    private readonly RestaurantScheduleStatus _scheduleStatus;

    public CannotCloseWhenRestaurantScheduleStatusIsClosedRule(RestaurantScheduleStatus scheduleStatus)
    {
        _scheduleStatus = scheduleStatus;
    }

    public Error Error => RestaurantErrorCodes.CannotCloseWhenRestaurantIsClosed;

    public bool IsBroken() => _scheduleStatus == RestaurantScheduleStatus.Closed;

    public static string Message => "Cannot close when restaurant schedule status is closed";
}
