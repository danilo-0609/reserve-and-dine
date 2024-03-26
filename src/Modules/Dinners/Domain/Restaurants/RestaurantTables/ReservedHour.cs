using Dinners.Domain.Common;

namespace Dinners.Domain.Restaurants.RestaurantTables;

public sealed record ReservedHour
{
    public DateTime ReservationDateTime { get; private set; }

    public TimeRange ReservationTimeRange { get; private set; }

    public int NumberOfTable {  get; private set; }

    public ReservedHour(DateTime reservationDateTime, TimeRange reservationTimeRange, int numberOfTable)
    {
        ReservationDateTime = reservationDateTime;
        ReservationTimeRange = reservationTimeRange;
        NumberOfTable = numberOfTable;
    }

    private ReservedHour() { }  
}
