namespace Dinners.Domain.Restaurants.RestaurantInformations;

public sealed record Speciality
{
    public string Value { get; private set; }

    public Speciality(string value)
    {
        Value = value;
    }

    private Speciality() { }

}
