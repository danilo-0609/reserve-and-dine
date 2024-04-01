namespace Dinners.Domain.Restaurants.RestaurantInformations;

public sealed record RestaurantInformation
{
    public string Title { get; private set; }

    public string Description { get; private set; }

    public string Type { get; private set; }

    public static RestaurantInformation Create(
        string title,
        string description,
        string type)
    { 
        return new RestaurantInformation(title,
            description,
            type);
    }

    private RestaurantInformation(string title,
        string description,
        string type)
    {
        Title = title;
        Description = description;
        Type = type;
    }

    private RestaurantInformation() { }
}
