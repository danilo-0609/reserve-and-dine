using BuildingBlocks.Domain.Events;

namespace Dinners.Domain.Restaurants.RestaurantRatings.Events;

public sealed record RatingPublishedDomainEvent(
    Guid DomainEventId,
    RestaurantRatingId Id,
    Guid ClientId,
    int Stars,
    RestaurantId RestaurantId,
    string RestaurantTitle,
    DateTime OcurredOn) : IDomainEvent;