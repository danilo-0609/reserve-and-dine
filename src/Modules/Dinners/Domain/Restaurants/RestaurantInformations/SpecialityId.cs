using BuildingBlocks.Domain.Entities;
using Newtonsoft.Json;

namespace Dinners.Domain.Restaurants.RestaurantInformations;

public sealed record SpecialityId : EntityId<Guid>
{
    public override Guid Value { get; protected set; }

    public static SpecialityId Create(Guid value) => new SpecialityId(value);

    public static SpecialityId CreateUnique() => new SpecialityId(Guid.NewGuid());

    [JsonConstructor]
    private SpecialityId(Guid value)
    {
        Value = value;
    }
}
