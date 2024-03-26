using Dinners.Domain.Common;

namespace Dinners.Domain.Restaurants.RestaurantTables;

public sealed record ReservedHour
{
    public DateTime ReservationDateTime { get; private set; }

    public TimeRange ReservationTimeRange { get; private set; }

    public int NumberOfTable {  get; private set; }

    public RestaurantId RestaurantId { get; private set; }

    public ReservedHour(DateTime reservationDateTime, TimeRange reservationTimeRange, int numberOfTable, RestaurantId restaurantId)
    {
        ReservationDateTime = reservationDateTime;
        ReservationTimeRange = reservationTimeRange;
        NumberOfTable = numberOfTable;
        RestaurantId = restaurantId;
    }

    private ReservedHour() { }  
}
