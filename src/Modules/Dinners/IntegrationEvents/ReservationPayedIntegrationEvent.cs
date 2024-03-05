using BuildingBlocks.Application;

namespace Dinners.IntegrationEvents;

public sealed record ReservationPayedIntegrationEvent(Guid IntegrationEventId,
    Guid ReservationPaymentId,
    Guid ReservationId,
    Guid ClientId,
    decimal Price,
    string Currency,
    DateTime OcurredOn) : IntegrationEvent(IntegrationEventId, OcurredOn);
