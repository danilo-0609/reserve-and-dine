using BuildingBlocks.Application;
using Dinners.Domain.Reservations.DomainEvents;

namespace Dinners.Application.Reservations.Request.DomainEvents;

internal sealed class ReservationRequestedDomainEventHandler : IDomainEventHandler<ReservationRequestedDomainEvent>
{

    public Task Handle(ReservationRequestedDomainEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
