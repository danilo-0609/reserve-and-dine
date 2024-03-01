using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Common;
using Dinners.Domain.Restaurants.RestaurantTables;
using ErrorOr;

namespace Dinners.Domain.Restaurants.Rules;

internal sealed class TableCannotBeReservedAtCertainTimeWhenTableIsReservedAtThatTimeNowRule : IBusinessRule
{
    private readonly List<RestaurantTable> _restaurantTables;
    private readonly DateTime _reservationRequestDateTime;
    private readonly TimeRange _reservationRequestTimeRange;

    public TableCannotBeReservedAtCertainTimeWhenTableIsReservedAtThatTimeNowRule(List<RestaurantTable> restaurantTables, 
        DateTime reservationRequestDateTime, 
        TimeRange reservationRequestTimeRange)
    {
        _restaurantTables = restaurantTables;
        _reservationRequestDateTime = reservationRequestDateTime;
        _reservationRequestTimeRange = reservationRequestTimeRange;
    }

    public Error Error => throw new NotImplementedException();

    public bool IsBroken() => _restaurantTables
        .Any(r => r.ReservedHours
            .ContainsKey(_reservationRequestDateTime)
        && r.ReservedHours.Values.Any(i => i.Start == _reservationRequestTimeRange.Start));

    public static string Message => "Table cannot be reserved at certain time when table is reserved at that time";
}
