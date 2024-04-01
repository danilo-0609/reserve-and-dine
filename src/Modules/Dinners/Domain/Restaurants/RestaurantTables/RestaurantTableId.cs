using BuildingBlocks.Domain.Entities;

namespace Dinners.Domain.Restaurants.RestaurantTables;

public sealed record RestaurantTableId : EntityId<Guid>
{
    public override Guid Value { get ; protected set ; }

    public static RestaurantTableId Create(Guid value) => new RestaurantTableId(value);

    public static RestaurantTableId CreateUnique() => new RestaurantTableId(Guid.NewGuid());

    private RestaurantTableId(Guid value)
    {
        Value = value;
    }
}
