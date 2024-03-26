using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Common;
using Dinners.Domain.Restaurants.Errors;
using Dinners.Domain.Restaurants.RestaurantTables;
using ErrorOr;

namespace Dinners.Domain.Restaurants.Rules;

internal sealed class TableCannotBeReservedAtCertainTimeWhenTableIsReservedAtThatTimeNowRule : IBusinessRule
{
    private readonly List<RestaurantTable> _restaurantTables;
    private readonly TimeRange _reservationRequestTimeRange;

    public TableCannotBeReservedAtCertainTimeWhenTableIsReservedAtThatTimeNowRule(List<RestaurantTable> restaurantTables,
        TimeRange reservationRequestTimeRange)
    {
        _restaurantTables = restaurantTables;
        _reservationRequestTimeRange = reservationRequestTimeRange;
    }

    public Error Error => RestaurantErrorCodes.CannotBeReservedAtCertainTimeWhenTableIsReservedAtThatTime;

    public bool IsBroken() => IsReservationTimeNotValid();

    private bool IsReservationTimeNotValid()
    {
        return _restaurantTables
                .Any(r => r.ReservedHours.Any(r => r.ReservationDateTime == _reservationRequestTimeRange.Start) ||
                r.ReservedHours.Any(r => r.ReservationTimeRange.Start < _reservationRequestTimeRange.End) &&
                r.ReservedHours.Any(r => r.ReservationTimeRange.End > _reservationRequestTimeRange.Start));
    }

    public static string Message => "Table cannot be reserved at certain time when table is reserved at that time";
}