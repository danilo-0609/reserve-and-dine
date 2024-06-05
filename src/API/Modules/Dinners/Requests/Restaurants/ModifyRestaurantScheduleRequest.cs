namespace API.Modules.Dinners.Requests.Restaurants;

public sealed record ModifyRestaurantScheduleRequest(DayOfWeek Day,
    string Start,
    string End);
