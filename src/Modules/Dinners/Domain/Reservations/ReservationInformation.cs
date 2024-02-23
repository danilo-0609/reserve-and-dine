using Dinners.Domain.Common;

namespace Dinners.Domain.Reservations;

public sealed record ReservationInformation
{
    public string ReservedTable { get; private set; } = string.Empty;

    public decimal ReservationPrice { get; private set; }

    public TimeRange TimeOfReservation { get; private set; }

    public DateTime ReservationDateTime { get; private set; }

    public int NumberOfDiners { get; private set; }

    public ReservationInformation(
        string reservedTable,
        decimal reservationPrice,
        TimeRange timeOfReservation,
        DateTime reservationDateTime,
        int numberOfDiners)
    {
        ReservedTable = reservedTable;
        ReservationPrice = reservationPrice;
        TimeOfReservation = timeOfReservation;
        ReservationDateTime = reservationDateTime;
        NumberOfDiners = numberOfDiners;
    }
}
