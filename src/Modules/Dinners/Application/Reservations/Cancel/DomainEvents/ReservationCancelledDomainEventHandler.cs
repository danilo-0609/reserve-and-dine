using BuildingBlocks.Application;
using Dinners.Domain.Reservations.DomainEvents;

namespace Dinners.Application.Reservations.Cancel.DomainEvents;

internal sealed class ReservationCancelledDomainEventHandler : IDomainEventHandler<ReservationCancelledDomainEvent>
{
    public Task Handle(ReservationCancelledDomainEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;       
    }
}
