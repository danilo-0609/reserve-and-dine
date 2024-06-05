namespace API.Modules.Dinners.Requests.Restaurants;

public sealed record RateRestaurantRequest(int Stars,
    string Comment = "");
