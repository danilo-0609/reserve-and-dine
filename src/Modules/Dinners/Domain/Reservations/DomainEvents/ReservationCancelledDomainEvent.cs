using BuildingBlocks.Domain.Events;

namespace Dinners.Domain.Reservations.DomainEvents;

public sealed record ReservationCancelledDomainEvent(
    Guid DomainEventId,
    ReservationId ReservationId,
    DateTime OcurredOn) : IDomainEvent;