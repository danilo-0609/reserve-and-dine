using Dinners.Domain.Common;

namespace Dinners.Application.Reservations;

public sealed record ReservationInformationResponse(int ReservedTable,
    Price ReservationPrice,
    Domain.Reservations.TimeRange TimeOfReservation,
    DateTime ReservationDateTime);
