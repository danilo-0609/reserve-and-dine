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
            if (_requestedTimeRange.Start.Hour > _restaurantSchedule.HoursOfOperation.Start.Hour && 
                _requestedTimeRange.Start.Day == _restaurantSchedule.HoursOfOperation.Start.Day)
            {
                return true;
            }

            return false;
        }

        return false;
    }
}
