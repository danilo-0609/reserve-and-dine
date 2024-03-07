namespace Dinners.Application.Restaurants.Post.Requests;

public sealed record RestaurantLocalizationRequest(string Country,
    string City,
    string Region,
    string Neighborhood,
    string Addresss,
    string LocalizationDetails);
