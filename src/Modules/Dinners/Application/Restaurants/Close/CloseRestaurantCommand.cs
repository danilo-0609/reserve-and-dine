using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Restaurants.Close;

public sealed record CloseRestaurantCommand(Guid RestaurantId) : ICommand<ErrorOr<Guid>>;
