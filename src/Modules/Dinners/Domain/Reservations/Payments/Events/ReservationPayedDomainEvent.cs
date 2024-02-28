using BuildingBlocks.Domain.Events;
using Dinners.Domain.Reservations.ReservationsPayments;

namespace Dinners.Domain.Reservations.Payments.Events;

public sealed record ReservationPayedDomainEvent(
    Guid DomainEventId,
    ReservationPaymentId ReservationPaymentId,
    ReservationId ReservationId,
    Guid ClientId,
    DateTime OcurredOn) : IDomainEvent;
