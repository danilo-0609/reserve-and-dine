using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants.RestaurantRatings.Events;
using Dinners.IntegrationEvents;

namespace Dinners.Application.Restaurants.Rate.Publish.Events;

internal sealed class RatingPublishedDomainEventHandler : IDomainEventHandler<RatingPublishedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public RatingPublishedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task Handle(RatingPublishedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _eventBus.PublishAsync(new RatingPublishedIntegrationEvent(notification.DomainEventId,
            notification.Id.Value,
            notification.ClientId,
            notification.Stars,
            notification.RestaurantId.Value,
            notification.RestaurantTitle,
            DateTime.UtcNow));
    }
}
