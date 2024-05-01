namespace API.Modules.Dinners.Requets;

public sealed record UpdateTableRequest(int Number,
    int Seats,
    bool IsPremium,
    decimal Price,
    string Currency);
