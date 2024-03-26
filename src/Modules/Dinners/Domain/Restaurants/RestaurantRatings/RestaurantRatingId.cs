using BuildingBlocks.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Dinners.Domain.Restaurants.RestaurantRatings;

public sealed record RestaurantRatingId : EntityId<Guid>
{
    [Key]
    public override Guid Value { get; protected set; }

    public static RestaurantRatingId Create(Guid value) => new RestaurantRatingId(value);

    public static RestaurantRatingId CreateUnique() => new RestaurantRatingId(Guid.NewGuid());

    private RestaurantRatingId(Guid value)
    {
        Value = value;
    }

    private RestaurantRatingId() { }
}
