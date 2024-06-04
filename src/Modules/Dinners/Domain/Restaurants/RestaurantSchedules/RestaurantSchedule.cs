using BuildingBlocks.Domain.Entities;
using Dinners.Domain.Common;

namespace Dinners.Domain.Restaurants.RestaurantSchedules;

public sealed class RestaurantSchedule : Entity<RestaurantScheduleId, Guid>
{
    public new RestaurantScheduleId Id { get; private set; }

    public RestaurantId RestaurantId { get; set; }

    public DayOfOperation Day { get; set; }

    public TimeRange HoursOfOperation { get; private set; }

    public DateTime? ReopeningTime { get; private set; }

    private RestaurantSchedule(RestaurantScheduleId id, 
        RestaurantId restaurantId, 
        DayOfOperation days, 
        TimeRange hoursOfOperation, 
        DateTime? reopeningTime)
    {
        Id = id;
        RestaurantId = restaurantId;
        Day = days;
        HoursOfOperation = hoursOfOperation;
        ReopeningTime = reopeningTime;
    }

    public static RestaurantSchedule Create(RestaurantId restaurantId,
        DayOfWeek day,
        TimeSpan start,
        TimeSpan end)
    {
        return new RestaurantSchedule(RestaurantScheduleId.CreateUnique(), restaurantId, new DayOfOperation(day), new TimeRange(start, end), null);
    }
    
    public void EstablishReopeningTime(DateTime reopeningTime)
    {
        ReopeningTime = reopeningTime;
    }

    private RestaurantSchedule() { }
}
