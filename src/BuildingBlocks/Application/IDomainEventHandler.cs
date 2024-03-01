using BuildingBlocks.Domain.Events;
using MediatR;

namespace BuildingBlocks.Application;

public interface IDomainEventHandler<in TNotification> : INotificationHandler<TNotification>
    where TNotification : IDomainEvent
{
}
