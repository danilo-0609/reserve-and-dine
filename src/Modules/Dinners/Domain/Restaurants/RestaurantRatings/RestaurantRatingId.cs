using BuildingBlocks.Domain.AggregateRoots;
using System.Text.Json.Serialization;

namespace Dinners.Domain.Restaurants.RestaurantRatings;

public sealed record RestaurantRatingId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    public static RestaurantRatingId Create(Guid value) => new RestaurantRatingId(value);

    public static RestaurantRatingId CreateUnique() => new RestaurantRatingId(Guid.NewGuid());

    [JsonConstructor]
    private RestaurantRatingId(Guid value)
    {
        Value = value;
    }

    private RestaurantRatingId() { }
}
