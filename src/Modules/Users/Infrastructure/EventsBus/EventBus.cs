using BuildingBlocks.Application;
using MassTransit;
using Users.Application.Common;

namespace Users.Infrastructure.EventsBus;

internal sealed class EventBus : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint;

    public async Task PublishAsync<T>(T @event) where T : IntegrationEvent
    {
        await _publishEndpoint.Publish(@event);
    }
}
