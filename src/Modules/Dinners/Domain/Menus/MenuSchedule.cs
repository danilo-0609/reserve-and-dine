using Dinners.Domain.Common;

namespace Dinners.Domain.Menus;

public sealed record MenuSchedule
{
    public List<string> Days { get; private set; }

    public TimeRange AvailableMenuHours { get; private set; }

    public MenuSchedule(List<string> days, TimeRange availableMenuHours)
    {
        Days = days;
        AvailableMenuHours = availableMenuHours;
    }
}
