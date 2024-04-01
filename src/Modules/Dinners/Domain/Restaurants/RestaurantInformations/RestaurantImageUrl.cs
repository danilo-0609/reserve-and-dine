using BuildingBlocks.Domain.Entities;

namespace Dinners.Domain.Restaurants.RestaurantInformations;

public sealed class RestaurantImageUrl : Entity<RestaurantImageUrlId, Guid>
{
    public new RestaurantImageUrlId Id { get; private set; }

    public RestaurantId RestaurantId { get; private set; }

    public string Value { get; private set; }

    public RestaurantImageUrl(RestaurantImageUrlId id, RestaurantId restaurantId, string value)
    {
        Id = id;
        RestaurantId = restaurantId;
        Value = value;
    }

    private RestaurantImageUrl() { }
}
