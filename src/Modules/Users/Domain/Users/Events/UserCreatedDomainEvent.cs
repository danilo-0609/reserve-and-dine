using BuildingBlocks.Domain.Events;

namespace Users.Domain.Users.Events;

public sealed record UserCreatedDomainEvent(
    Guid DomainEventId,
    UserId UserId,
    DateTime OcurredOn) : IDomainEvent;