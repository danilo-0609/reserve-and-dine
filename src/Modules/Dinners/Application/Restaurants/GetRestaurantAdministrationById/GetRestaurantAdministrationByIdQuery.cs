using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Restaurants.GetRestaurantAdministrationById;

public sealed record GetRestaurantAdministrationByIdQuery(Guid RestaurantId) : IQuery<ErrorOr<IReadOnlyList<RestaurantAdministrationResponse>>>;
