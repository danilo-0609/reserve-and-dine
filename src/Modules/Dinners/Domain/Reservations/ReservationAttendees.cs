namespace Dinners.Domain.Reservations;

public sealed record ReservationAttendees
{
    public Guid ClientId { get; private set; }

    public string Name { get; private set; }

    public int NumberOfAttendees { get; private set; }

    public static ReservationAttendees Create(
        Guid clientId, 
        string name, 
        int numberOfAttendees) => new ReservationAttendees(clientId, name, numberOfAttendees);

    public void UpdateAttendees(int numberOfAttendees)
    {
        NumberOfAttendees = numberOfAttendees;
    }
    
    private ReservationAttendees(Guid clientId, string name, int numberOfAttendees)
    {
        ClientId = clientId;
        Name = name;
        NumberOfAttendees = numberOfAttendees;
    }

    private ReservationAttendees() { }
}
