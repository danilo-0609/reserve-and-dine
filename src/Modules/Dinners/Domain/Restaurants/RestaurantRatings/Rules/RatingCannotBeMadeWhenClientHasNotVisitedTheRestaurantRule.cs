using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Restaurants.RestaurantRatings.Events;
using ErrorOr;

namespace Dinners.Domain.Restaurants.RestaurantRatings.Rules;

internal sealed class RatingCannotBeMadeWhenClientHasNotVisitedTheRestaurantRule : IBusinessRule
{
    private readonly bool _hasVisited;

    public RatingCannotBeMadeWhenClientHasNotVisitedTheRestaurantRule(bool hasVisited)
    {
        _hasVisited = hasVisited;
    }

    public Error Error => RestaurantRatingErrorsCodes.CannotRateWhenHasNotVisitedTheRestaurant;

    public bool IsBroken() => _hasVisited == false;

    public static string Message => "Rating cannot be made when client has not visited the restaurant yet";
}
