using BuildingBlocks.Domain.Entities;
using Newtonsoft.Json;

namespace Dinners.Domain.Menus.Details;

public sealed record TagId : EntityId<Guid>
{
    public override Guid Value { get; protected set; }

    public static TagId Create(Guid value) => new TagId(value);

    public static TagId CreateUnique() => new TagId(Guid.NewGuid());

    [JsonConstructor]
    private TagId(Guid value)
    {
        Value = value;
    }
}
