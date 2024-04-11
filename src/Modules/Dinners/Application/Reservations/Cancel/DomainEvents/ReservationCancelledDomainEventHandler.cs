using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Reservations.DomainEvents;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Domain.Restaurants;

namespace Dinners.Application.Reservations.Cancel.DomainEvents;

internal sealed class ReservationCancelledDomainEventHandler : IDomainEventHandler<ReservationCancelledDomainEvent>
{
    public Task Handle(ReservationCancelledDomainEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;       
    }
}
