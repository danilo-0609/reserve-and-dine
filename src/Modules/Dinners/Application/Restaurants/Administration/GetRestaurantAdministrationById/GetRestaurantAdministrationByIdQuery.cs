using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Restaurants.Administration.GetRestaurantAdministrationById;

public sealed record GetRestaurantAdministrationByIdQuery(Guid RestaurantId) : IQuery<ErrorOr<IReadOnlyList<RestaurantAdministrationResponse>>>;
