namespace API.Modules.Dinners.Requests.Restaurants;

public sealed record UpdateTableRequest(int Number,
    int Seats,
    bool IsPremium,
    decimal Price,
    string Currency);
