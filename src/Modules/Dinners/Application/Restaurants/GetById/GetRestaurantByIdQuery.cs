using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Restaurants.GetById;

public sealed record GetRestaurantByIdQuery(Guid RestaurantId) : IQuery<ErrorOr<RestaurantResponse>>;
