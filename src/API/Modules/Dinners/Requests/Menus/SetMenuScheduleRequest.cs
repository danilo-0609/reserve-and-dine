namespace API.Modules.Dinners.Requests.Menus;

public sealed record SetMenuScheduleRequest(DayOfWeek Day,
    TimeSpan Start,
    TimeSpan End);
