using BuildingBlocks.Application;

namespace Users.Application.Common;

public interface IEventBus
{
    Task PublishAsync<T>(T @event)
        where T : IntegrationEvent;
}
