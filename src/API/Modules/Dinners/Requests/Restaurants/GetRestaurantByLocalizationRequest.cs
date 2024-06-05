namespace API.Modules.Dinners.Requests.Restaurants;

public sealed record GetRestaurantByLocalizationRequest(string Country,
    string City,
    string Region,
    string? Neighborhood = null);
