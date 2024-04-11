using BuildingBlocks.Domain.AggregateRoots;
using Newtonsoft.Json;

namespace Dinners.Domain.Restaurants;

public sealed record RestaurantId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    public static RestaurantId Create(Guid value) => new RestaurantId(value);

    public static RestaurantId CreateUnique() => new RestaurantId(Guid.NewGuid());

    [JsonConstructor]
    private RestaurantId(Guid value) : base(value)  
    {
        Value = value;
    }

    private RestaurantId() { }
}
