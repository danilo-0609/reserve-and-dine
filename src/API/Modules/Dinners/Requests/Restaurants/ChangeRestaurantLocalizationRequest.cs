namespace API.Modules.Dinners.Requests.Restaurants;

public sealed record ChangeRestaurantLocalizationRequest(string Country,
    string City,
    string Region,
    string Neighborhood,
    string Address,
    string LocalizationDetails = "");
