using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Restaurants.Errors;
using Dinners.Domain.Restaurants.RestaurantSchedules;
using ErrorOr;

namespace Dinners.Domain.Restaurants.Rules;

internal sealed class CannotOpenWhenRestaurantScheduleStatusIsOpenedRule : IBusinessRule
{
    private readonly RestaurantScheduleStatus _scheduleStatus;

    public CannotOpenWhenRestaurantScheduleStatusIsOpenedRule(RestaurantScheduleStatus scheduleStatus)
    {
        _scheduleStatus = scheduleStatus;
    }

    public Error Error => RestaurantErrorCodes.CannotOpenWhenRestaurantIsOpened;

    public bool IsBroken() => _scheduleStatus == RestaurantScheduleStatus.Open;

    public static string Message => "Cannot open when restaurant schedule status is opened";
}
