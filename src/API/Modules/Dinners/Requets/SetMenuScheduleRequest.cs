namespace API.Modules.Dinners.Requets;

public sealed record SetMenuScheduleRequest(DayOfWeek Day,
    TimeSpan Start,
    TimeSpan End);
