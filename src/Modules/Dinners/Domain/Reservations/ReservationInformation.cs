using Dinners.Domain.Common;

namespace Dinners.Domain.Reservations;

public sealed record ReservationInformation
{
    public int ReservedTable { get; private set; }

    public TimeRange TimeOfReservation { get; private set; }

    public DateTime ReservationDateTime { get; private set; }


    public static ReservationInformation Create(int reservedTable,  
        TimeSpan start,
        TimeSpan end,
        DateTime reservationDateTime)
    {
        return new ReservationInformation(
            reservedTable, 
            new TimeRange(start, end),
            reservationDateTime);
    }

    private ReservationInformation(
        int reservedTable,
        TimeRange timeOfReservation,
        DateTime reservationDateTime)
    {
        ReservedTable = reservedTable;
        TimeOfReservation = timeOfReservation;
        ReservationDateTime = reservationDateTime;
    }

    private ReservationInformation() { }
}
