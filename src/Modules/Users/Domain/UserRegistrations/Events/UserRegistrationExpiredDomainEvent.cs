using BuildingBlocks.Domain.Events;

namespace Users.Domain.UserRegistrations.Events;

public record UserRegistrationExpiredDomainEvent(
    Guid DomainEventId,
    UserRegistrationId Id,
    DateTime OcurredOn) : IDomainEvent;