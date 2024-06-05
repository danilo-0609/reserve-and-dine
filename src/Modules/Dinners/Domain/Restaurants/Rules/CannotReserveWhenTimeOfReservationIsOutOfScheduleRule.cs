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
    private readonly DateTime _reservationDateTimeRequested;

    public CannotReserveWhenTimeOfReservationIsOutOfScheduleRule(RestaurantSchedule restaurantSchedule,
        TimeRange reservationTimeRangeRequested,
        DateTime reservationDateTimeRequested)
    {
        _restaurantSchedule = restaurantSchedule;
        _reservationTimeRangeRequested = reservationTimeRangeRequested;
        _reservationDateTimeRequested = reservationDateTimeRequested;
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
        var openingTime = _restaurantSchedule.HoursOfOperation.Start;
        var closingTime = _restaurantSchedule.HoursOfOperation.End;

        var reservationStart = _reservationTimeRangeRequested.Start;
        var reservationEnd = _reservationTimeRangeRequested.End;

        bool isSameDay = openingTime < closingTime;
        bool isOvernight = !isSameDay;

        if (isSameDay)
        {
            return reservationStart >= openingTime && reservationEnd <= closingTime;
        }
        
        // Handling overnight case
        if (reservationStart >= openingTime || reservationEnd <= closingTime)
        {
            return true;
        }

        // Handle edge case where the reservation end time is past midnight of the next day
        var nextDay = _reservationDateTimeRequested.AddDays(1).DayOfWeek;
        
        if (isOvernight 
            && _restaurantSchedule.Day.DayOfWeek == nextDay
            && reservationEnd <= closingTime)
        {
            return true;
        }

        return false;
    }

    public static string Message => "Cannot reserve when time requested is out of restaurant schedule, because the restaurant will be closed";
}
