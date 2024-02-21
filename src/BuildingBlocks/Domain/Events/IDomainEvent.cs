using MediatR;

namespace BuildingBlocks.Domain.Events;

public interface IDomainEvent : INotification
{
    Guid DomainEventId { get; }

    DateTime OcurredOn { get; }
}
