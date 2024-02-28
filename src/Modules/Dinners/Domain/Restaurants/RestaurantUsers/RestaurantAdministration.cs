namespace Dinners.Domain.Restaurants.RestaurantUsers;

public sealed record RestaurantAdministration
{
    public string Name { get; private set; }

    public Guid AdministratorId { get; private set; }

    public RestaurantId RestaurantId { get; private set; }

    public string AdministratorTitle { get; private set; }

    private RestaurantAdministration(Guid administratorId, RestaurantId restaurantId, string name, string administratorTitle)
    {
        AdministratorId = administratorId;
        RestaurantId = restaurantId;
        Name = name;
        AdministratorTitle = administratorTitle;
    }

    public static RestaurantAdministration Create(string name,
        Guid administratorId,
        RestaurantId restaurantId,
        string administratorTitle)
    {
        return new RestaurantAdministration(administratorId, restaurantId, name, administratorTitle);
    }
}
