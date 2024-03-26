using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Common;
using Dinners.Domain.Restaurants.Errors;
using Dinners.Domain.Restaurants.RestaurantSchedules;
using ErrorOr;

namespace Dinners.Domain.Restaurants.Rules;

internal sealed class CannotReserveWhenTimeOfReservationIsOutOfScheduleRule : IBusinessRule
{
    private readonly RestaurantSchedule _restaurantSchedule;
    private readonly TimeRange _reservationTimeRangeRequested;

    public CannotReserveWhenTimeOfReservationIsOutOfScheduleRule(RestaurantSchedule restaurantSchedule, 
        TimeRange reservationTimeRangeRequested)
    {
        _restaurantSchedule = restaurantSchedule;
        _reservationTimeRangeRequested = reservationTimeRangeRequested;
    }

    public Error Error => RestaurantErrorCodes.CannotReserveWhenTimeOfReservationIsOutOfSchedule;

    public bool IsBroken()
    {
        if (IsRestaurantOpenForReservation())
        {
            return false;
        }

        return true;
    }

    public bool IsRestaurantOpenForReservation()
    {
        if (_restaurantSchedule.Day.DayOfWeek == _reservationTimeRangeRequested.Start.DayOfWeek
            && _restaurantSchedule.HoursOfOperation.Start <= _reservationTimeRangeRequested.Start
            && _restaurantSchedule.HoursOfOperation.End > _reservationTimeRangeRequested.End)
        {
            return true;
        }

        return false;
    }

    public static string Message => "Cannot reserve when time requested is out of restaurant schedule, because the restaurant will be closed";
}
