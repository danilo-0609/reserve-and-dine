using BuildingBlocks.Domain.Entities;

namespace Dinners.Domain.Restaurants.RestaurantUsers;

public sealed class RestaurantAdministration : Entity<RestaurantAdministrationId, Guid>
{
    public new RestaurantAdministrationId Id { get; private set; }

    public RestaurantId RestaurantId { get; private set; }  

    public string Name { get; private set; }

    public Guid AdministratorId { get; private set; }

    public string AdministratorTitle { get; private set; }

    private RestaurantAdministration(RestaurantAdministrationId id, RestaurantId restaurantId, Guid administratorId, string name, string administratorTitle)
    {
        Id = id;
        RestaurantId = restaurantId;
        AdministratorId = administratorId;
        Name = name;
        AdministratorTitle = administratorTitle;
    }

    public static RestaurantAdministration Create(RestaurantId restaurantId,
        string name,
        Guid administratorId,
        string administratorTitle)
    {
        return new RestaurantAdministration(RestaurantAdministrationId.CreateUnique(), restaurantId, administratorId, name, administratorTitle);
    }

    public RestaurantAdministration Update(string name,
        Guid administratorId,
        string administratorTitle)
    {
        return new RestaurantAdministration(Id, RestaurantId, administratorId, name, administratorTitle);
    }

    private RestaurantAdministration() { }
}
