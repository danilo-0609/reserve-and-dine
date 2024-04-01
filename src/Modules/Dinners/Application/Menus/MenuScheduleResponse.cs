namespace Dinners.Application.Menus;

public sealed record MenuScheduleResponse(DayOfWeek Day,
    TimeSpan StartTime,
    TimeSpan EndTime);
