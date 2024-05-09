using BuildingBlocks.Domain.Events;

namespace Users.Domain.UserRegistrations.Events;

public sealed record UserRegistrationConfirmedDomainEvent(
    Guid DomainEventId,
    UserRegistrationId Id,
    DateTime OcurredOn) : IDomainEvent;
