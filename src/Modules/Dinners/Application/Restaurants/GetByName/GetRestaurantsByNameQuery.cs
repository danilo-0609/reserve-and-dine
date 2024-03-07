using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Restaurants.GetByName;

public sealed record GetRestaurantsByNameQuery(string Name) : IQuery<ErrorOr<IReadOnlyList<RestaurantResponse>>>;