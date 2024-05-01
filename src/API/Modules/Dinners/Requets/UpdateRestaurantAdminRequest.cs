namespace API.Modules.Dinners.Requets;

public sealed record UpdateRestaurantAdminRequest(string AdministratorTitle, 
    string Name, 
    Guid AdministratorId);
