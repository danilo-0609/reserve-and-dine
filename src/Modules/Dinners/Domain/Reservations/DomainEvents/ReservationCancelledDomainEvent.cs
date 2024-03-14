using BuildingBlocks.Domain.Events;
using Dinners.Domain.Restaurants;

namespace Dinners.Domain.Reservations.DomainEvents;

public sealed record ReservationCancelledDomainEvent(
    Guid DomainEventId,
    ReservationId ReservationId,
    RestaurantId RestaurantId,
    int NumberOfTable,
    string causeOfCancellation,
    DateTime ReservationDateTime,
    DateTime OcurredOn) : IDomainEvent;