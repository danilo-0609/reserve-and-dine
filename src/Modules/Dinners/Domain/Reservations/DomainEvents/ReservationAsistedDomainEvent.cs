using BuildingBlocks.Domain.Events;
using Dinners.Domain.Menus;

namespace Dinners.Domain.Reservations.DomainEvents;

public sealed record ReservationAsistedDomainEvent(
    Guid DomainEventId,
    ReservationId ReservationId,
    List<MenuId?> MenuIds,
    DateTime OcurredOn) : IDomainEvent;
