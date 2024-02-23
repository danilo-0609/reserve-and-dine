namespace Dinners.Domain.Restaurants;

public sealed record RestaurantLocalization
{
    public string Country { get; private set; }

    public string City { get; private set; }

    public string Region { get; private set; }

    public string Neighborhood { get; private set; }

    public string Addresss { get; private set; }

    public string LocalizationDetails { get; private set; } = string.Empty;

    private RestaurantLocalization(
        string country, 
        string city, 
        string region, 
        string neighborhood, 
        string addresss, 
        string localizationDetails)
    {
        Country = country;
        City = city;
        Region = region;
        Neighborhood = neighborhood;
        Addresss = addresss;
        LocalizationDetails = localizationDetails;
    }

    public static RestaurantLocalization Create(string country,
        string city,
        string region,
        string neighborhood,
        string address,
        string localizationDetails)
    {
        return new RestaurantLocalization(country,
            city,
            region,
            neighborhood,
            address,
            localizationDetails);
    }

    private RestaurantLocalization() { }
}
