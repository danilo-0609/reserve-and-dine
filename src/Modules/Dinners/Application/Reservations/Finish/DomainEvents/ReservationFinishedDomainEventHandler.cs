using BuildingBlocks.Application;
using Dinners.Domain.Reservations.DomainEvents;

namespace Dinners.Application.Reservations.Finish.DomainEvents;

internal sealed class ReservationFinishedDomainEventHandler : IDomainEventHandler<ReservationFinishedDomainEvent>
{
    public Task Handle(ReservationFinishedDomainEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
