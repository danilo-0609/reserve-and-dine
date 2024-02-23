using BuildingBlocks.Domain.Events;

namespace Dinners.Domain.Restaurants.Events;

public sealed record NewRestaurantPostedDomainEvent(
    Guid DomainEventId,
    RestaurantId RestaurantId,
    DateTime OcurredOn) : IDomainEvent;
