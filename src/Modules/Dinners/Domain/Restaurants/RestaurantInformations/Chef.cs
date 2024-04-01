using BuildingBlocks.Domain.Entities;

namespace Dinners.Domain.Restaurants.RestaurantInformations;

public sealed class Chef : Entity<ChefId, Guid>
{
    public new ChefId Id { get; private set; }

    public RestaurantId RestaurantId { get; private set; }

    public string Value { get; private set; }

    public Chef(ChefId id, RestaurantId restaurantId, string value)
    {
        Id = id;
        RestaurantId = restaurantId;
        Value = value;
    }

    private Chef() { }

}
