using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Restaurants.Open;

public sealed record OpenRestaurantCommand(Guid RestaurantId) : ICommand<ErrorOr<Guid>>;
