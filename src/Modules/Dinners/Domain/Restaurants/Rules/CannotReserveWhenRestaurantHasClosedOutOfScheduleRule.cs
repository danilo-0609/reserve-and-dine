using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Common;
using Dinners.Domain.Restaurants.Errors;
using Dinners.Domain.Restaurants.RestaurantSchedules;
using ErrorOr;

namespace Dinners.Domain.Restaurants.Rules;

internal sealed class CannotReserveWhenRestaurantHasClosedOutOfScheduleRule : IBusinessRule
{
    private readonly RestaurantSchedule _restaurantSchedule;
    private readonly RestaurantScheduleStatus _scheduleStatus;
    private readonly TimeRange _requestedTimeRange;

    public CannotReserveWhenRestaurantHasClosedOutOfScheduleRule(RestaurantSchedule restaurantSchedule,
        RestaurantScheduleStatus scheduleStatus,
        TimeRange requestedTimeRange)
    {
        _restaurantSchedule = restaurantSchedule;
        _scheduleStatus = scheduleStatus;
        _requestedTimeRange = requestedTimeRange;
    }

    public Error Error => RestaurantErrorCodes.CannotReserveWhenRestaurantHasClosedOutOfSchedule;

    public bool IsBroken() => IsReservationTimeOutOfRestaurantSchedule();

    public static string Message => "Cannot reserve when restaurant has closed out of schedule";


    private bool IsReservationTimeOutOfRestaurantSchedule()
    {
        if (_scheduleStatus ==  RestaurantScheduleStatus.Closed)
        {
            if (_requestedTimeRange.Start.Hours > _restaurantSchedule.HoursOfOperation.Start.Hours && 
                _requestedTimeRange.Start.Days == _restaurantSchedule.HoursOfOperation.Start.Days)
            {
                return true;
            }

            return false;
        }

        return false;
    }
}
