namespace Dinners.Domain.Restaurants.RestaurantInformations;

public sealed record Chef
{
    public string Value { get; private set; }

    public Chef(string value)
    {
        Value = value;
    }

    private Chef() { }

}
