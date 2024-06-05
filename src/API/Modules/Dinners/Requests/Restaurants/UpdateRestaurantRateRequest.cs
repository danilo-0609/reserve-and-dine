namespace API.Modules.Dinners.Requests.Restaurants;

public sealed record UpdateRestaurantRateRequest(int Stars,
    string Comment);
