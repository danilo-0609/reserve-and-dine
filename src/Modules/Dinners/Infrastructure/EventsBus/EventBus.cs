using BuildingBlocks.Application;
using Dinners.Application.Common;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Dinners.Infrastructure.EventsBus;

internal sealed class EventBus : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<EventBus> _logger;

    public EventBus(IPublishEndpoint publishEndpoint, ILogger<EventBus> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T @event) 
        where T : IntegrationEvent
    {
        _logger.LogInformation("Publishing {@Name}. At {@DateTime}",
            nameof(@event),
            DateTime.Now);

        await _publishEndpoint.Publish(@event);
    }
}
