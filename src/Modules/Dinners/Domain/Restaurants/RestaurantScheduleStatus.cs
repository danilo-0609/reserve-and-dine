namespace Dinners.Domain.Restaurants;

public sealed record RestaurantScheduleStatus
{
    public string Value { get; private set; }

    public RestaurantScheduleStatus Opened => new RestaurantScheduleStatus(nameof(Opened));

    public RestaurantScheduleStatus Closed => new RestaurantScheduleStatus(nameof(Closed));

    public RestaurantScheduleStatus(string value)
    {
        Value = value;
    }
}
