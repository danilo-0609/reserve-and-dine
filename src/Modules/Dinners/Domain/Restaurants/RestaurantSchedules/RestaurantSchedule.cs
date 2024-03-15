using Dinners.Domain.Common;

namespace Dinners.Domain.Restaurants.RestaurantSchedules;

public sealed record RestaurantSchedule
{
    public List<DayOfWeek> Days { get; set; }

    public TimeRange HoursOfOperation { get; private set; }

    private RestaurantSchedule(List<DayOfWeek> days, TimeRange hoursOfOperation)
    {
        Days = days;
        HoursOfOperation = hoursOfOperation;
    }

    public static RestaurantSchedule Create(List<DayOfWeek> days,
        TimeSpan start,
        TimeSpan end)
    {
        return new RestaurantSchedule(days, new TimeRange(start, end));
    }

    private RestaurantSchedule() { }
}
