using Dinners.Domain.Common;

namespace Dinners.Domain.Reservations;

public sealed record ReservationInformation
{
    public int ReservedTable { get; private set; }

    public Price ReservationPrice { get; private set; }

    public TimeRange TimeOfReservation { get; private set; }

    public DateTime ReservationDateTime { get; private set; }


    public static ReservationInformation Create(int reservedTable,
        decimal price,
        string currency,
        TimeSpan start,
        TimeSpan end,
        DateTime reservationDateTime)
    {
        return new ReservationInformation(
            reservedTable, 
            new Price(price, currency), 
            new TimeRange(start, end),
            reservationDateTime);
    }

    private ReservationInformation(
        int reservedTable,
        Price reservationPrice,
        TimeRange timeOfReservation,
        DateTime reservationDateTime)
    {
        ReservedTable = reservedTable;
        ReservationPrice = reservationPrice;
        TimeOfReservation = timeOfReservation;
        ReservationDateTime = reservationDateTime;
    }

    private ReservationInformation() { }
}
