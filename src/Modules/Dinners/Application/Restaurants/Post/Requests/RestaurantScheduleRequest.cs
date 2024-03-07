namespace Dinners.Application.Restaurants.Post.Requests;

public sealed record RestaurantScheduleRequest(List<DayOfWeek> Days,
    TimeSpan Start,
    TimeSpan End);
