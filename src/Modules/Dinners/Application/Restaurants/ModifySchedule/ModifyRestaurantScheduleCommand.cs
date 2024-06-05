using Dinners.Application.Common;
using ErrorOr;
namespace Dinners.Application.Restaurants.ModifySchedule;

public sealed record ModifyRestaurantScheduleCommand(Guid RestaurantId,
    DayOfWeek Day,
    string Start,
    string End) : ICommand<ErrorOr<Success>>;
