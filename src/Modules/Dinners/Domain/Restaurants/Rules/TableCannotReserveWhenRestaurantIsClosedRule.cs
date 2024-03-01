using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Common;
using Dinners.Domain.Restaurants.Errors;
using Dinners.Domain.Restaurants.RestaurantSchedules;
using ErrorOr;

namespace Dinners.Domain.Restaurants.Rules;

internal sealed class TableCannotReserveWhenRestaurantIsClosedRule : IBusinessRule
{
    private readonly RestaurantSchedule _restaurantSchedule;
    private readonly TimeRange _reservationTimeRangeRequested;
    private readonly DateTime _reservationDateTimeRequested;

    public TableCannotReserveWhenRestaurantIsClosedRule(RestaurantSchedule restaurantSchedule, 
        TimeRange reservationTimeRangeRequested, 
        DateTime reservationDateTimeRequested)
    {
        _restaurantSchedule = restaurantSchedule;
        _reservationTimeRangeRequested = reservationTimeRangeRequested;
        _reservationDateTimeRequested = reservationDateTimeRequested;
    }

    public Error Error => RestaurantErrorCodes.CannotReserveWhenRestaurantIsClosed;

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
        if (_restaurantSchedule.Days.Contains(_reservationDateTimeRequested.DayOfWeek)
            && _restaurantSchedule.HoursOfOperation.Start <= _reservationTimeRangeRequested.Start
            && _restaurantSchedule.HoursOfOperation.End > _reservationTimeRangeRequested.End)
        {
            return true;
        }

        return false;
    }

    public static string Message => "Cannot reserve when restaurant is closed";
}
