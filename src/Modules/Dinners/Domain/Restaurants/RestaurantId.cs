using BuildingBlocks.Domain.AggregateRoots;

namespace Dinners.Domain.Restaurants;

public sealed record RestaurantId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    public static RestaurantId Create(Guid value) => new RestaurantId(value);

    public static RestaurantId CreateUnique() => new RestaurantId(Guid.NewGuid());

    public RestaurantId(Guid value)
    {
        Value = value;
    }
}
