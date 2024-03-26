namespace Dinners.Domain.Reservations;

public sealed record ReservationStatus
{
    public string Value { get; private set; }

    public static ReservationStatus Finished => new ReservationStatus(nameof(Finished));

    public static ReservationStatus Cancelled => new ReservationStatus(nameof(Cancelled));

    public static ReservationStatus Visiting => new ReservationStatus(nameof(Visiting));

    public static ReservationStatus Requested => new ReservationStatus(nameof(Requested));
    
    public static ReservationStatus Paid => new ReservationStatus(nameof(Paid));

    private ReservationStatus(string value)
    {
        Value = value;
    }

    private ReservationStatus() { }
}
