using Dinners.Domain.Restaurants.RestaurantRatings.Rules;
using ErrorOr;

namespace Dinners.Domain.Restaurants.RestaurantRatings.Events;

public static class RestaurantRatingErrorsCodes
{
    public static Error CannotRateWhenHasNotVisitedTheRestaurant =>
        Error.Validation("RestaurantRatings.CannotRate", RatingCannotBeMadeWhenClientHasNotVisitedTheRestaurantRule.Message);
}
