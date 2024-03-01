using BuildingBlocks.Domain.Events;

namespace Dinners.Domain.Reservations.DomainEvents;

public sealed record ReservationRequestedDomainEvent(
    Guid DomainEventId,
    ReservationId ReservationId,
    Guid ClientId,
    int TableNumber,
    DateTime OcurredOn) : IDomainEvent;
