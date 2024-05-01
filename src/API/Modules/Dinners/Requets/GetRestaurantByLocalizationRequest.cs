namespace API.Modules.Dinners.Requets;

public sealed record GetRestaurantByLocalizationRequest(string Country,
    string City,
    string Region,
    string? Neighborhood = null);
