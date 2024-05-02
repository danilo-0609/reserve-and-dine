using Dinners.Domain.Common;

namespace Dinners.Application.Reservations;

public sealed record ReservationResponse(Guid Id,
    ReservationInformationResponse ReservationInformation,
    Guid RestaurantId,
    ReservationAttendeesResponse ReservationAttendees,
    string ReservationStatus,
    Guid? PaymentId,
    List<Guid> MenuIds);
