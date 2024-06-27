using BuildingBlocks.Application;

namespace Dinners.IntegrationEvents;

public sealed record NewRestaurantAdministratorAddedIntegrationEvent(Guid IntegrationEventId,
    Guid UserId,
    DateTime OccurredOn) : IntegrationEvent(IntegrationEventId, OccurredOn);
