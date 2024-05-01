namespace API.Modules.Dinners.Requets;

public sealed record ModifyRestaurantScheduleRequest(DayOfWeek Day,
    DateTime Start,
    DateTime End);
