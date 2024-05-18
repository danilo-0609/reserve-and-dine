using BuildingBlocks.Domain.Events;

namespace Dinners.Domain.Restaurants.RestaurantUsers.Events;

public sealed record NewRestaurantAdministratorAddedDomainEvent(Guid DomainEventId,
    Guid UserId,
    DateTime OcurredOn) : IDomainEvent;
