using BuildingBlocks.Domain.Events;

namespace Users.Domain.UserRegistrations.Events;

public record NewUserRegisteredDomainEvent(
    Guid DomainEventId,
    UserRegistrationId Id,
    string Login,
    string Email,
    DateTime OcurredOn) : IDomainEvent;