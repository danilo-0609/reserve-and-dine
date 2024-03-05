namespace Dinners.Application.Reservations;

public sealed record ReservationAttendeesResponse(Guid ClientId,
    string Name,
    int NumberOfAttendees);
