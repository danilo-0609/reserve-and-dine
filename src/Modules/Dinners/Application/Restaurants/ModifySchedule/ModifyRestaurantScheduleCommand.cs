using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.ModifySchedule;

public sealed record ModifyRestaurantScheduleCommand(Guid RestaurantId,
    List<DayOfWeek> Days,
    TimeSpan Start,
    TimeSpan End) : ICommand<ErrorOr<Unit>>;
