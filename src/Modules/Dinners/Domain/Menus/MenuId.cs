using BuildingBlocks.Domain.AggregateRoots;
using Newtonsoft.Json;

namespace Dinners.Domain.Menus;

public sealed record MenuId : AggregateRootId<Guid>
{
    public override Guid Value { get ; protected set; }

    public static MenuId CreateUnique() => new MenuId(Guid.NewGuid());

    public static MenuId Create(Guid id) => new MenuId(id);

    [JsonConstructor]
    private MenuId(Guid value)
    {
        Value = value;
    }

    private MenuId() { }
}
