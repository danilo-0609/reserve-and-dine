using BuildingBlocks.Domain.Events;
using Dinners.Domain.Menus;
using Dinners.Domain.Restaurants;

namespace Dinners.Domain.Reservations.DomainEvents;

public sealed record ReservationVisitedDomainEvent(
    Guid DomainEventId,
    ReservationId ReservationId,
    RestaurantId RestaurantId,
    Guid ClientId,
    List<MenuId> MenuIds,
    DateTime OcurredOn) : IDomainEvent;
