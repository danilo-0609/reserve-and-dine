using BuildingBlocks.Domain.Entities;

namespace Dinners.Domain.Menus.Details;

public sealed record MenuImageUrlId : EntityId<Guid>
{
    public override Guid Value { get; protected set; }

    public static MenuImageUrlId Create(Guid value) => new MenuImageUrlId(value);

    public static MenuImageUrlId CreateUnique() => new MenuImageUrlId(Guid.NewGuid());

    private MenuImageUrlId(Guid value)
    {
        Value = value;
    }
}
