using BuildingBlocks.Application;

namespace Dinners.IntegrationEvents;

public sealed record MoneyRefundedIntegrationEvent(Guid IntegrationEventId,
    Guid RefundId,
    Guid ReservationId,
    Guid ClientId,
    decimal Price,
    string Currency,
    DateTime OcurredOn) : IntegrationEvent(IntegrationEventId, OcurredOn);
