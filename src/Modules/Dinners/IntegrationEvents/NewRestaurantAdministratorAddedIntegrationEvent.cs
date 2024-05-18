using BuildingBlocks.Application;

namespace Dinners.IntegrationEvents;

public sealed record NewRestaurantAdministratorAddedIntegrationEvent(Guid IntegrationEventId,
    Guid UserId,
    DateTime OcurredOn) : IntegrationEvent(IntegrationEventId, OcurredOn);
