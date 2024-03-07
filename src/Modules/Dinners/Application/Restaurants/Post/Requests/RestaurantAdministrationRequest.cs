namespace Dinners.Application.Restaurants.Post.Requests;

public sealed record RestaurantAdministrationRequest(string Name,
        Guid AdministratorId,
        string AdministratorTitle);
