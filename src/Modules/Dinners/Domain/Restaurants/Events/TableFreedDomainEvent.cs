using BuildingBlocks.Domain.Events;

namespace Dinners.Domain.Restaurants.Events;

public sealed record TableFreedDomainEvent(
    Guid DomainEventId,
    RestaurantId RestaurantId,
    int TableNumber,
    DateTime OcurredOn) : IDomainEvent;
