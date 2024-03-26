using BuildingBlocks.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Dinners.Domain.Menus.MenuReviews;

public sealed record MenuReviewId : EntityId<Guid>
{
    [Key]
    public override Guid Value { get; protected set; }

    public static MenuReviewId Create(Guid menuReviewId) => new MenuReviewId(menuReviewId);

    public static MenuReviewId CreateUnique() => new MenuReviewId(Guid.NewGuid());

    private MenuReviewId(Guid value)
    {
        Value = value;
    }

    private MenuReviewId() { }
}
