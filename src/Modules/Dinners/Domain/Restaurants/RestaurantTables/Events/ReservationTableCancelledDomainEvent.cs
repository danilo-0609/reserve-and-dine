using BuildingBlocks.Domain.Events;

namespace Dinners.Domain.Restaurants.RestaurantTables.Events;

public sealed record ReservationTableCancelledDomainEvent(Guid DomainEventId,
    RestaurantId RestaurantId,
    DateTime ReservationDateTime,
    DateTime OcurredOn) : IDomainEvent;
