namespace API.Modules.Dinners.Requets;

public sealed record AddTableRequest(int Number,
    int Seats,
    bool IsPremium,
    decimal Price,
    string Currency);
