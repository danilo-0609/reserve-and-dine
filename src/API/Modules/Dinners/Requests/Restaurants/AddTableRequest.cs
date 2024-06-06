namespace API.Modules.Dinners.Requests.Restaurants;

public sealed record AddTableRequest(int Number,
    int Seats,
    bool IsPremium);
