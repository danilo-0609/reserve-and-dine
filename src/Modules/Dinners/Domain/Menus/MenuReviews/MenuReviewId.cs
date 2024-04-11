using BuildingBlocks.Domain.AggregateRoots;
using Newtonsoft.Json;

namespace Dinners.Domain.Menus.MenuReviews;

public sealed record MenuReviewId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    public static MenuReviewId Create(Guid menuReviewId) => new MenuReviewId(menuReviewId);

    public static MenuReviewId CreateUnique() => new MenuReviewId(Guid.NewGuid());

    [JsonConstructor]
    private MenuReviewId(Guid value) : base(value)
    {
        Value = value;
    }

    private MenuReviewId() { }
}
