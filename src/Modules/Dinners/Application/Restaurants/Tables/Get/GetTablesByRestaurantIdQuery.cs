using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Restaurants.Tables.Get;

public sealed record GetTablesByRestaurantIdQuery(Guid RestaurantId) : IQuery<ErrorOr<List<RestaurantTableResponse>>>;
