using BuildingBlocks.Domain.Events;
using Dinners.Domain.Common;

namespace Dinners.Domain.Reservations.Refunds.Events;

public sealed record MoneyRefundedDomainEvent(
    Guid DomainEventId,
    RefundId RefundId,
    ReservationId ReservationId,
    Guid ClientId,
    Price RefundedMoney,
    DateTime OcurredOn) : IDomainEvent;
