using BuildingBlocks.Domain.Events;

namespace Dinners.Domain.Restaurants.Events;

public sealed record TableReservedDomainEvent(
    Guid DomainEventId,
    RestaurantId RestaurantId,
    int TableNumberReserved,
    DateTime OcurredOn) : IDomainEvent;
