using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants.Events;
using Dinners.IntegrationEvents;

namespace Dinners.Application.Restaurants.Post.DomainEvents;

internal sealed class NewRestaurantPostedDomainEventHandler : IDomainEventHandler<NewRestaurantPostedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public NewRestaurantPostedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task Handle(NewRestaurantPostedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _eventBus.PublishAsync(new NewRestaurantPostedIntegrationEvent(notification.DomainEventId,
            notification.RestaurantId.Value,
            notification.ClientId,
            notification.OcurredOn));
    }
}
