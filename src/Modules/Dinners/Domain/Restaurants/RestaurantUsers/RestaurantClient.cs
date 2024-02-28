namespace Dinners.Domain.Restaurants.RestaurantUsers;

public sealed record RestaurantClient
{
    public RestaurantId RestaurantId { get; private set; }

    public Guid ClientId { get; private set; }

    public int NumberOfVisits { get; private set; }

    public static RestaurantClient Create(RestaurantId restaurantId,
        Guid clientId,
        int numberOfVisits)
    {
        return new RestaurantClient(restaurantId, clientId, numberOfVisits);
    }

    private RestaurantClient(RestaurantId restaurantId, Guid clientId, int numberOfVisits)
    {
        RestaurantId = restaurantId;
        ClientId = clientId;
        NumberOfVisits = numberOfVisits;
    }

    private RestaurantClient() { }
}
