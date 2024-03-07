using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Restaurants.Tables.Get;

internal sealed record GetTablesByRestaurantIdQuery(Guid RestaurantId) : IQuery<ErrorOr<List<RestaurantTableResponse>>>;
