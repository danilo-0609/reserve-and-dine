namespace Dinners.Application.Menus;

public sealed record MenuScheduleResponse(List<DayOfWeek> DayOfWeeks,
    TimeSpan StartTime,
    TimeSpan EndTime);
