namespace API.Modules.Dinners.Requests.Restaurants;

public sealed record UpdateRestaurantInformationRequest(string Title,
    string Description,
    string Type);
