namespace Dinners.Domain.Restaurants.RestaurantInformations;

public sealed record RestaurantImageUrl
{
    public string Value { get; private set; }

    public RestaurantImageUrl(string value)
    {
        Value = value;
    }

    private RestaurantImageUrl() { }
}
