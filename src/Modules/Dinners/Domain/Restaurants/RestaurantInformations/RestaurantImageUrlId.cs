using BuildingBlocks.Domain.Entities;

namespace Dinners.Domain.Restaurants.RestaurantInformations;

public sealed record RestaurantImageUrlId : EntityId<Guid>
{
    public override Guid Value { get; protected set; }

    public static RestaurantImageUrlId Create(Guid value) => new RestaurantImageUrlId(value);

    public static RestaurantImageUrlId CreateUnique() => new RestaurantImageUrlId(Guid.NewGuid());

    private RestaurantImageUrlId(Guid value)
    {
        Value = value;
    }
}
