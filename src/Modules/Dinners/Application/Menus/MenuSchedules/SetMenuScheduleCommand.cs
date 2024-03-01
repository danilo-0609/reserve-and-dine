using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Menus.MenuSchedules;

public sealed record SetMenuScheduleCommand(Guid MenuId,
    List<DayOfWeek> DayOfWeeks,
    TimeSpan Open,
    TimeSpan Close) : ICommand<ErrorOr<Unit>>;
