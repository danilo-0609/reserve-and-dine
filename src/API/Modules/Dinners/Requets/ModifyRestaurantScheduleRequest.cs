namespace API.Modules.Dinners.Requets;

public sealed record ModifyRestaurantScheduleRequest(DayOfWeek Day,
    TimeSpan Start,
    TimeSpan End);
