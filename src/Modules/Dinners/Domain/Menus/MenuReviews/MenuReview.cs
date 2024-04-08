using BuildingBlocks.Domain.AggregateRoots;
using Dinners.Domain.Menus.MenuReviews.Events;
using Dinners.Domain.Menus.MenuReviews.Rules;
using ErrorOr;

namespace Dinners.Domain.Menus.MenuReviews;

public sealed class MenuReview : AggregateRoot<MenuReviewId, Guid>
{
    public new MenuReviewId Id { get; private set; }

    public decimal Rate { get; private set; }

    public Guid ClientId { get; private set; }

    public string Comment { get; private set; } = string.Empty;

    public DateTime ReviewedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }


    public static ErrorOr<MenuReview> Post(MenuId menuId,
        Guid clientId,
        decimal rate,
        List<MenuConsumer> menuConsumers,
        DateTime reviewedAt,
        string comment = "")
    {
        MenuReview menuReview = new MenuReview(
            MenuReviewId.CreateUnique(),
            rate,
            clientId,
            comment,
            reviewedAt,
            null);

        var mustHaveConsumedTheMenuBeforeRule = menuReview.CheckRule(new MenuCannotBeReviewedWhenUserHasNotConsumedItRule(menuConsumers, clientId));

        if (mustHaveConsumedTheMenuBeforeRule.IsError)
        {
            return mustHaveConsumedTheMenuBeforeRule.FirstError;
        }

        var rateMustBeANumberBetweenZeroToFiveRule = menuReview.CheckRule(new MenuReviewMustBeANumberBetweenZeroToFiveRule(rate));

        if (rateMustBeANumberBetweenZeroToFiveRule.IsError)
        {
            return rateMustBeANumberBetweenZeroToFiveRule.FirstError;
        }

        menuReview.AddDomainEvent(new MenuReviewedDomainEvent(Guid.NewGuid(),
            menuId,
            menuReview.Id,
            DateTime.UtcNow));

        return menuReview;
    }

    public ErrorOr<MenuReview> ModifyRate(
        decimal rate,
        DateTime updatedAt)
    {
        var rateMustBeANumberBetweenZeroToFiveRule = CheckRule(new MenuReviewMustBeANumberBetweenZeroToFiveRule(rate));

        if (rateMustBeANumberBetweenZeroToFiveRule.IsError)
        {
            return rateMustBeANumberBetweenZeroToFiveRule.FirstError;
        }

        return new MenuReview(Id, rate, ClientId, Comment, ReviewedAt, updatedAt);
    }

    public MenuReview ModifyComment(
        string comment,
        DateTime updatedAt)
    {
        return new MenuReview(Id, Rate, ClientId, comment, ReviewedAt, updatedAt);
    }

    private MenuReview(
        MenuReviewId id, 
        decimal rate, 
        Guid clientId, 
        string comment, 
        DateTime reviewedAt,
        DateTime? updatedAt)
    {
        Id = id;
        Rate = rate;
        ClientId = clientId;
        Comment = comment;
        ReviewedAt = reviewedAt;
        UpdatedAt = updatedAt;
    }

    private MenuReview() { }
}

