using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Restaurants.Rate.GetByRestaurantId;

public sealed record GetRateByRestaurantIdQuery(Guid RestaurantId) : IQuery<ErrorOr<List<RatingResponse>>>;
