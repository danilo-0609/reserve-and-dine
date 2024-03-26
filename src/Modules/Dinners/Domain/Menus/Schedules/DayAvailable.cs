namespace Dinners.Domain.Menus.Schedules;

public sealed record DayAvailable
{
    public DayOfWeek DayOfWeek { get; set; }

    public DayAvailable(DayOfWeek dayOfWeek)
    {
        DayOfWeek = dayOfWeek;
    }

    private DayAvailable() { }
}
