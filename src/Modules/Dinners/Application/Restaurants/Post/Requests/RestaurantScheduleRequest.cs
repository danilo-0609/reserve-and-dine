namespace Dinners.Application.Restaurants.Post.Requests;

public sealed record RestaurantScheduleRequest(DayOfWeek Day,
    string Start,
    string End);
