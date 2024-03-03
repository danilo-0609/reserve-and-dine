using BuildingBlocks.Domain.Events;
using Dinners.Domain.Restaurants;

namespace Dinners.Domain.Reservations.DomainEvents;

public sealed record ReservationFinishedDomainEvent(
    Guid DomainEventId,
    ReservationId ReservationId,
    RestaurantId RestaurantId,
    Guid ClientId,
    int NumberOfTable, 
    DateTime OcurredOn) : IDomainEvent;
