using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Restaurants.Rate.Publish;

public sealed record RateRestaurantCommand(Guid RestaurantId,
    int Stars,
    string Comment = "") : ICommand<ErrorOr<Guid>>;
