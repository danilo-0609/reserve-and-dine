using BuildingBlocks.Domain.Entities;
using Newtonsoft.Json;

namespace Dinners.Domain.Restaurants.RestaurantInformations;

public sealed record RestaurantImageUrlId : EntityId<Guid>
{
    public override Guid Value { get; protected set; }

    public static RestaurantImageUrlId Create(Guid value) => new RestaurantImageUrlId(value);

    public static RestaurantImageUrlId CreateUnique() => new RestaurantImageUrlId(Guid.NewGuid());

    [JsonConstructor]
    private RestaurantImageUrlId(Guid value)
    {
        Value = value;
    }
}
