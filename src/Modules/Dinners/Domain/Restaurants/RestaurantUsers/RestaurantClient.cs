using BuildingBlocks.Domain.Entities;

namespace Dinners.Domain.Restaurants.RestaurantUsers;

public sealed class RestaurantClient : Entity<RestaurantClientId, Guid>
{
    public new RestaurantClientId Id { get; private set; }  

    public RestaurantId RestaurantId { get; private set; }

    public Guid ClientId { get; private set; }

    public int NumberOfVisits { get; private set; }

    public static RestaurantClient Create(RestaurantId restaurantId,
        Guid clientId,
        int numberOfVisits)
    {
        return new RestaurantClient(RestaurantClientId.CreateUnique(), restaurantId, clientId, numberOfVisits);
    }

    public void AddVisit()
    {
        NumberOfVisits = NumberOfVisits + 1;
    }

    private RestaurantClient(RestaurantClientId id, RestaurantId restaurantId, Guid clientId, int numberOfVisits)
    {
        Id = id;
        RestaurantId = restaurantId;
        ClientId = clientId;
        NumberOfVisits = numberOfVisits;
    }

    private RestaurantClient() { }
}
