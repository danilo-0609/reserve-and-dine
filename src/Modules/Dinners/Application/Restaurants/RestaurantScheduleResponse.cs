namespace Dinners.Application.Restaurants;

public sealed record RestaurantScheduleResponse(DayOfWeek Day,
    DateTime Open,
    DateTime Close,
    DateTime? ReopeningTime);
