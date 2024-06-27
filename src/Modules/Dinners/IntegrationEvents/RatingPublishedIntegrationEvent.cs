using BuildingBlocks.Application;

namespace Dinners.IntegrationEvents;

public sealed record RatingPublishedIntegrationEvent(
    Guid IntegrationEventId,
    Guid RestaurantRatingId,
    Guid ClientId,
    int Stars,
    Guid RestaurantId,
    string RestaurantTitle,
    DateTime OccurredOn) : IntegrationEvent(IntegrationEventId, OccurredOn);
