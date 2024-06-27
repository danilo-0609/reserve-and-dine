using BuildingBlocks.Domain.Entities;
using Newtonsoft.Json;

namespace Dinners.Domain.Restaurants.RestaurantInformations;

public sealed record SpecialtyId : EntityId<Guid>
{
    public override Guid Value { get; protected set; }

    public static SpecialtyId Create(Guid value) => new SpecialtyId(value);

    public static SpecialtyId CreateUnique() => new SpecialtyId(Guid.NewGuid());

    [JsonConstructor]
    private SpecialtyId(Guid value)
    {
        Value = value;
    }
}
