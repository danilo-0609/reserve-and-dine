using BuildingBlocks.Domain.Events;

namespace Dinners.Domain.Restaurants.Events;

public sealed record NewRestaurantPostedDomainEvent(
    Guid DomainEventId,
    RestaurantId RestaurantId,
    Guid ClientId,
    DateTime OcurredOn) : IDomainEvent;
