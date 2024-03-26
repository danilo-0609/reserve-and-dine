namespace Dinners.Domain.Restaurants.RestaurantSchedules;

public sealed record DayOfOperation
{
    public DayOfWeek DayOfWeek { get; private set; }

    public DayOfOperation(DayOfWeek dayOfWeek)
    {
        DayOfWeek = dayOfWeek;
    }

    private DayOfOperation() { }
}
