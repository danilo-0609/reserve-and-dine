namespace Dinners.Application.Restaurants.Post.Requests;

public sealed record RestaurantInformationRequest(string Title,
    string Description,
    string Type);
