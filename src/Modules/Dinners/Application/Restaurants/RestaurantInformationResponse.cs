namespace Dinners.Application.Restaurants;

public sealed record RestaurantInformationResponse(string Title,
    string Description,
    string Type,
    IReadOnlyList<string>? Chefs,
    IReadOnlyList<string>? Specialties);
