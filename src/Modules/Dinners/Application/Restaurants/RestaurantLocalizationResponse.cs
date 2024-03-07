namespace Dinners.Application.Restaurants;

public sealed record RestaurantLocalizationResponse(string Country,
    string City,
    string Region,
    string Neighborhood,
    string Addresss,
    string LocalizationDetails);
