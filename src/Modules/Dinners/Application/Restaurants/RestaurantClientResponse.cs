namespace Dinners.Application.Restaurants;

public sealed record RestaurantClientResponse(Guid ClientId,
    int NumberOfVisits);
