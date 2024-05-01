namespace API.Modules.Dinners.Requets;

public sealed record AddRestaurantAdminRequest(string Name,
    string AdministratorTitle,
    Guid NewAdministratorId);
