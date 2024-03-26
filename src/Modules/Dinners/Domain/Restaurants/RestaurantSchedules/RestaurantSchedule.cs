using Dinners.Domain.Common;

namespace Dinners.Domain.Restaurants.RestaurantSchedules;

public sealed record RestaurantSchedule
{
    public DayOfOperation Day { get; set; }

    public TimeRange HoursOfOperation { get; private set; }

    public DateTime? ReopeningTime { get; private set; }

    private RestaurantSchedule(DayOfOperation days, TimeRange hoursOfOperation, DateTime? reopeningTime)
    {
        Day = days;
        HoursOfOperation = hoursOfOperation;
        ReopeningTime = reopeningTime;
    }

    public static RestaurantSchedule Create(DayOfWeek day,
        DateTime start,
        DateTime end)
    {
        return new RestaurantSchedule(new DayOfOperation(day), new TimeRange(start, end), null);
    }
    
    public void EstablishReopeningTime(DateTime reopeningTime)
    {
        ReopeningTime = reopeningTime;
    }

    private RestaurantSchedule() { }
}
