namespace API.Modules.Dinners.Requets;

public sealed record RateRestaurantRequest(int Stars,
    string Comment = "");
