using Dinners.Domain.Common;

namespace Dinners.Application.Reservations.Refunds;

public sealed record RefundResponse(Guid RefundId,
    Guid ReservationId,
    Guid ClientId,
    Price Price, 
    DateTime RefundedAt);
