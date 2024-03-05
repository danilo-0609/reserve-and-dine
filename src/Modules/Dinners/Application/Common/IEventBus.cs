using BuildingBlocks.Application;

namespace Dinners.Application.Common;

public interface IEventBus
{
    Task PublishAsync<T>(T @event)
        where T : IntegrationEvent;
}
