using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants.RestaurantUsers.Events;
using Dinners.IntegrationEvents;

namespace Dinners.Application.Restaurants.Administration.Add.Events;

internal sealed class NewRestaurantAdministratorAddedEventHandler : IDomainEventHandler<NewRestaurantAdministratorAddedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public NewRestaurantAdministratorAddedEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task Handle(NewRestaurantAdministratorAddedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await _eventBus.PublishAsync(new NewRestaurantAdministratorAddedIntegrationEvent(domainEvent.DomainEventId,
            domainEvent.UserId,
            domainEvent.OcurredOn));
    }
}