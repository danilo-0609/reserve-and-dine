namespace Dinners.Application.Reservations;

public sealed record ReservationInformationResponse(int ReservedTable,
    Domain.Reservations.TimeRange TimeOfReservation,
    DateTime ReservationDateTime);
