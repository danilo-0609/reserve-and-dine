namespace API.Modules.Dinners.Requests.Restaurants;

public sealed record AddRestaurantAdminRequest(string Name,
    string AdministratorTitle,
    Guid NewAdministratorId);
