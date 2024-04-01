using BuildingBlocks.Domain.Entities;

namespace Dinners.Domain.Menus.Schedules;

public sealed class MenuSchedule : Entity<MenuScheduleId, Guid>
{
    public new MenuScheduleId Id { get; private set; }

    public MenuId MenuId { get; private set; }

    public DayOfWeek Day { get; private set; }

    public TimeSpan StartTimeSpan { get; private set; }

    public TimeSpan EndTimeSpan { get; private set; }

    public static MenuSchedule Create(DayOfWeek day, TimeSpan startTimeSpan, TimeSpan endTimeSpan, MenuId menuId)
    {
        return new MenuSchedule(MenuScheduleId.CreateUnique(), day, startTimeSpan, endTimeSpan, menuId);
    }

    private MenuSchedule(MenuScheduleId id, DayOfWeek day, TimeSpan startTimeSpan, TimeSpan endTimeSpan, MenuId menuId)
    {
        Id = id;
        MenuId = menuId;
        Day = day;
        StartTimeSpan = startTimeSpan;
        EndTimeSpan = endTimeSpan;
    }

    private MenuSchedule() { }
}
