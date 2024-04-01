using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Menus.MenuSchedules;

public sealed record SetMenuScheduleCommand(Guid MenuId,
    DayOfWeek Day,
    TimeSpan Start,
    TimeSpan End) : ICommand<ErrorOr<Unit>>;
