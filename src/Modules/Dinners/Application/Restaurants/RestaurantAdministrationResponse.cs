namespace Dinners.Application.Restaurants;

public sealed record RestaurantAdministrationResponse(string Name,
    Guid AdministratorId,
    string AdministratorTitle);
