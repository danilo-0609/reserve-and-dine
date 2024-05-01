namespace API.Modules.Dinners.Requets;

public sealed record UpdateRestaurantInformationRequest(string Title,
    string Description,
    string Type);
