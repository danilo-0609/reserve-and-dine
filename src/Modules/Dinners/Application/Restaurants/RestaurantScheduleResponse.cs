namespace Dinners.Application.Restaurants;

public sealed record RestaurantScheduleResponse(List<DayOfWeek> Days,
    TimeSpan Open,
    TimeSpan Close);
