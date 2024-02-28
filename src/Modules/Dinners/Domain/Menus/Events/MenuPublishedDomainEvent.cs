using BuildingBlocks.Domain.Events;

namespace Dinners.Domain.Menus.Events;

public sealed record MenuPublishedDomainEvent(
    Guid DomainEventId,
    MenuId MenuId,
    DateTime OcurredOn) : IDomainEvent;
