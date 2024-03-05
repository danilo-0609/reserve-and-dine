namespace BuildingBlocks.Application;

public abstract record IntegrationEvent(Guid IntegrationEventId, DateTime OcurredOn);
