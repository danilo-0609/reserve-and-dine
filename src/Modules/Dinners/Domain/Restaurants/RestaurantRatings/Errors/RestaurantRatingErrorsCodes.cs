using Dinners.Domain.Restaurants.RestaurantRatings.Rules;
using ErrorOr;

namespace Dinners.Domain.Restaurants.RestaurantRatings.Events;

public static class RestaurantRatingErrorsCodes
{
    public static Error NotFoundByRestaurantId =>
        Error.NotFound("RestaurantRating.NotFoundByRestaurantId", "Ratings were not found");

    public static Error NotFound =>
        Error.NotFound("RestaurantRating.NotFound", "Restaurant rating was not found");

    public static Error CannotRateWhenHasNotVisitedTheRestaurant =>
        Error.Validation("RestaurantRatings.CannotRate", RatingCannotBeMadeWhenClientHasNotVisitedTheRestaurantRule.Message);

    public static Error IsNotRaterUser =>
        Error.Validation("RestaurantRatings.IsNotRaterUser", CannotUpdateRatingWhenIsNotRaterUserRule.Message);

    public static Error CannotDeleteRating =>
        Error.Unauthorized("RestaurantRatings.CannotDeleteRating", "Cannot delete rating if you're not the same rater user");
}
