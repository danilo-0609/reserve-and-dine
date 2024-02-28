using BuildingBlocks.Domain.Events;

namespace Dinners.Domain.Restaurants.Events;

public sealed record TableReservationCancelledDomainEvent(
    Guid DomainEventId,
    RestaurantId RestaurantId,
    int TableNumber,
    DateTime OcurredOn) : IDomainEvent;
