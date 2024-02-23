using Dinners.Domain.Common;

namespace Dinners.Domain.Restaurants;

public sealed record RestaurantSchedule
{
    private readonly List<string> _days;

    public IReadOnlyList<string> Days => _days.AsReadOnly();

    public TimeRange HoursOfOperation { get; private set; }

    private RestaurantSchedule(List<string> days, TimeRange hoursOfOperation)
    {
        _days = days;
        HoursOfOperation = hoursOfOperation;
    }

    public static RestaurantSchedule Create(List<string> days,
        TimeSpan start,
        TimeSpan end)
    {
        return new RestaurantSchedule(days, new TimeRange(start, end));
    }
}
