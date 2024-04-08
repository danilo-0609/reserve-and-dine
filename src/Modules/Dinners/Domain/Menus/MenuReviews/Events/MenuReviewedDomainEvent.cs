using BuildingBlocks.Domain.Events;

namespace Dinners.Domain.Menus.MenuReviews.Events;

public sealed record MenuReviewedDomainEvent(
    Guid DomainEventId,
    MenuId MenuId,
    MenuReviewId MenuReviewId,
    DateTime OcurredOn) : IDomainEvent;
     