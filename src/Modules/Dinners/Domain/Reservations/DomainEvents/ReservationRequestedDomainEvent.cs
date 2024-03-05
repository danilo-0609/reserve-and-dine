using BuildingBlocks.Domain.Events;
using Dinners.Domain.Common;
using Dinners.Domain.Restaurants;

namespace Dinners.Domain.Reservations.DomainEvents;

public sealed record ReservationRequestedDomainEvent(
    Guid DomainEventId,
    ReservationId ReservationId,
    RestaurantId RestaurantId,
    Guid ClientId,
    int TableNumber,
    TimeRange ReservationTimeRange,
    DateTime ReservationDateTimeRequested,
    DateTime OcurredOn) : IDomainEvent;
