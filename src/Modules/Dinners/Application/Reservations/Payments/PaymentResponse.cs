using Dinners.Domain.Common;

namespace Dinners.Application.Reservations.Payments;

public sealed record PaymentResponse(Guid Id,
    Guid PayerId,
    Price Price,
    DateTime PayedAt);
