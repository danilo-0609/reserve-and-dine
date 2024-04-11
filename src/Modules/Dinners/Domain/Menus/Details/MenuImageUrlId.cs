using BuildingBlocks.Domain.Entities;
using Newtonsoft.Json;

namespace Dinners.Domain.Menus.Details;

public sealed record MenuImageUrlId : EntityId<Guid>
{
    public override Guid Value { get; protected set; }

    public static MenuImageUrlId Create(Guid value) => new MenuImageUrlId(value);

    public static MenuImageUrlId CreateUnique() => new MenuImageUrlId(Guid.NewGuid());

    [JsonConstructor]
    private MenuImageUrlId(Guid value)
    {
        Value = value;
    }
}
