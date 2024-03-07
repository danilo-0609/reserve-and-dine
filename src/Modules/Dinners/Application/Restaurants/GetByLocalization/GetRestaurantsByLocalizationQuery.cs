using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Restaurants.GetByLocalization;

public sealed record GetRestaurantsByLocalizationQuery(string Country,
    string City,
    string Region,
    string? Neighborhood) : IQuery<ErrorOr<IReadOnlyList<RestaurantResponse>>>;
