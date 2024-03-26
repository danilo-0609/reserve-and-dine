using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.ModifySchedule;

public sealed record ModifyRestaurantScheduleCommand(Guid RestaurantId,
    DayOfWeek Day,
    DateTime Start,
    DateTime End) : ICommand<ErrorOr<Unit>>;
