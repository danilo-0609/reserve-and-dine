using Dinners.Domain.Common;

namespace Dinners.Domain.Menus;

public sealed record MenuSchedule
{
    public List<DayOfWeek> Days { get; private set; }

    public TimeRange AvailableMenuHours { get; private set; }

    public static MenuSchedule Create(List<DayOfWeek> days, TimeSpan start, TimeSpan end)
    {
        return new MenuSchedule(days, new TimeRange(start, end));
    }


    private MenuSchedule(List<DayOfWeek> days, TimeRange availableMenuHours)
    {
        Days = days;
        AvailableMenuHours = availableMenuHours;
    }

    private MenuSchedule() { }
}
