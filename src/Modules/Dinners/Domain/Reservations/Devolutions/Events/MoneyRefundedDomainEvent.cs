using BuildingBlocks.Domain.Events;

namespace Dinners.Domain.Reservations.Devolutions.Events;

public sealed record MoneyRefundedDomainEvent(
    Guid DomainEventId,
    DevolutionId DevolutionId,
    ReservationId ReservationId,
    DateTime OcurredOn) : IDomainEvent;
