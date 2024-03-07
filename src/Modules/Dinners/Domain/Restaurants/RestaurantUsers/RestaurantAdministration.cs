namespace Dinners.Domain.Restaurants.RestaurantUsers;

public sealed record RestaurantAdministration
{
    public string Name { get; private set; }

    public Guid AdministratorId { get; private set; }

    public string AdministratorTitle { get; private set; }

    private RestaurantAdministration(Guid administratorId, string name, string administratorTitle)
    {
        AdministratorId = administratorId;
        Name = name;
        AdministratorTitle = administratorTitle;
    }

    public static RestaurantAdministration Create(string name,
        Guid administratorId,
        string administratorTitle)
    {
        return new RestaurantAdministration(administratorId, name, administratorTitle);
    }

    public RestaurantAdministration Update(string name,
        Guid administratorId,
        string administratorTitle)
    {
        return new RestaurantAdministration(administratorId, name, administratorTitle);
    }
}
