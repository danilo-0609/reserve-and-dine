using BuildingBlocks.Domain.Events;

namespace Dinners.Domain.Restaurants.RestaurantRatings.Events;

public sealed record RatingPublishedDomainEvent(
    Guid DomainEventId,
    RestaurantRatingId Id,
    DateTime OcurredOn) : IDomainEvent;