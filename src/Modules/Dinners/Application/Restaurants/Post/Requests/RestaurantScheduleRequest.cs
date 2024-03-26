namespace Dinners.Application.Restaurants.Post.Requests;

public sealed record RestaurantScheduleRequest(DayOfWeek Day,
    DateTime Start,
    DateTime End);
