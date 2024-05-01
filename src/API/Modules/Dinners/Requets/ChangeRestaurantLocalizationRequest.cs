namespace API.Modules.Dinners.Requets;

public sealed record ChangeRestaurantLocalizationRequest(string Country,
    string City,
    string Region,
    string Neighborhood,
    string Address,
    string LocalizationDetails = "");
