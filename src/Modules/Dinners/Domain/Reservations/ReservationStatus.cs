namespace Dinners.Domain.Reservations;

public sealed record ReservationStatus
{
    public string Value { get; private set; }

    public ReservationStatus Cancelled => new ReservationStatus(nameof(Cancelled));

    public static ReservationStatus Confirmed => new ReservationStatus(nameof(Confirmed));

    public static ReservationStatus Requested => new ReservationStatus(nameof(Requested));
    
    public static ReservationStatus Payed => new ReservationStatus(nameof(Payed));

    public ReservationStatus(string value)
    {
        Value = value;
    }
}
