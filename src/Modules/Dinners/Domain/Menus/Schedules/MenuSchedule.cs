namespace Dinners.Domain.Menus.Schedules;

public sealed record MenuSchedule
{
    public List<DayAvailable> Days { get; private set; }

    public TimeRange AvailableMenuHours { get; private set; }

    public static MenuSchedule Create(List<DayOfWeek> days, TimeSpan start, TimeSpan end)
    {
        return new MenuSchedule(days.ConvertAll(day => new DayAvailable(day)), new TimeRange(start, end));
    }


    private MenuSchedule(List<DayAvailable> days, TimeRange availableMenuHours)
    {
        Days = days;
        AvailableMenuHours = availableMenuHours;
    }

    private MenuSchedule() { }
}
