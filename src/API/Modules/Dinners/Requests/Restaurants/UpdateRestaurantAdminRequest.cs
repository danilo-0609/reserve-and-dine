namespace API.Modules.Dinners.Requests.Restaurants;

public sealed record UpdateRestaurantAdminRequest(string AdministratorTitle,
    string Name,
    Guid AdministratorId);
