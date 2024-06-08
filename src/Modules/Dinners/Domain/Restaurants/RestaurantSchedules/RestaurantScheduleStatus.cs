namespace Dinners.Domain.Restaurants.RestaurantSchedules;

public sealed record RestaurantScheduleStatus
{
    public string Value { get; private set; }

    public static RestaurantScheduleStatus Open => new RestaurantScheduleStatus(nameof(Open));

    public static RestaurantScheduleStatus Closed => new RestaurantScheduleStatus(nameof(Closed));

    private RestaurantScheduleStatus(string value)
    {
        Value = value;
    }

    private RestaurantScheduleStatus() { }
}
