using Dinners.Domain.Common;

namespace Dinners.Application.Reservations;

public sealed record ReservationInformationResponse(int ReservedTable,
    Price ReservationPrice,
    TimeRange TimeOfReservation,
    DateTime ReservationDateTime);
