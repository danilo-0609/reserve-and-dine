namespace Dinners.Application.Restaurants;

public sealed record RestaurantScheduleResponse(DayOfWeek Day,
    TimeSpan Open,
    TimeSpan Close,
    DateTime? ReopeningTime);
