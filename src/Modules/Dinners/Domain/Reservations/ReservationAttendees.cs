namespace Dinners.Domain.Reservations;

public sealed record ReservationAttendees
{
    public Guid ClientId { get; private set; }

    public string Name { get; private set; }

    public int NumberOfAttendess { get; private set; }

    public static ReservationAttendees Create(
        Guid clientId, 
        string name, 
        int numberOfAttendees) => new ReservationAttendees(clientId, name, numberOfAttendees);

    public void UpdateAttendees(int numberOfAttendees)
    {
        NumberOfAttendess = numberOfAttendees;
    }
    
    private ReservationAttendees(Guid clientId, string name, int numberOfAttendess)
    {
        ClientId = clientId;
        Name = name;
        NumberOfAttendess = numberOfAttendess;
    }

    private ReservationAttendees() { }
}
