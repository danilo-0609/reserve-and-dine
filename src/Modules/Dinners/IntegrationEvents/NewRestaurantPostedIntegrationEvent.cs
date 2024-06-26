﻿using BuildingBlocks.Application;

namespace Dinners.IntegrationEvents;

public sealed record NewRestaurantPostedIntegrationEvent(Guid IntegrationEventId,
    Guid RestaurantId,
    Guid ClientId,
    DateTime OccurredOn) : IntegrationEvent(IntegrationEventId, OccurredOn);
