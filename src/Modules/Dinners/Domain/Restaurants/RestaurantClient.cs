namespace Dinners.Domain.Restaurants;

public sealed record RestaurantClient
{
    public RestaurantId RestaurantId { get; private set; }

    public Guid ClientId { get; private set; }

    public bool HasVisited { get; private set; }

    public RestaurantClient(RestaurantId restaurantId, Guid clientId, bool hasVisited)
    {
        RestaurantId = restaurantId;
        ClientId = clientId;
        HasVisited = hasVisited;
    }
}
