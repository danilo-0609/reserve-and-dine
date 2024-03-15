using BuildingBlocks.Domain.Entities;

namespace Dinners.Domain.Restaurants.RestaurantRatings;

public sealed record RestaurantRatingId : EntityId<Guid>
{
    public override Guid Value { get; protected set; }

    public static RestaurantRatingId Create(Guid value) => new RestaurantRatingId(value);

    public static RestaurantRatingId CreateUnique() => new RestaurantRatingId(Guid.NewGuid());

    private RestaurantRatingId(Guid value)
    {
        Value = value;
    }

    private RestaurantRatingId() { }
}
