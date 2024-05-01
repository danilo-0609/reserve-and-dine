using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Restaurants.RestaurantRatings.Events;
using ErrorOr;

namespace Dinners.Domain.Restaurants.RestaurantRatings.Rules;

internal sealed class CannotUpdateRatingWhenIsNotRaterUserRule : IBusinessRule
{
    private readonly Guid _clientId;
    private readonly Guid _ratingUserId;

    public CannotUpdateRatingWhenIsNotRaterUserRule(Guid clientId, Guid ratingUserId)
    {
        _clientId = clientId;
        _ratingUserId = ratingUserId;
    }

    public Error Error => RestaurantRatingErrorsCodes.CannotUpdateRateWhenIsNotUserRater;

    public bool IsBroken() => _clientId != _ratingUserId;

    public static string Message => "Cannot update rating when is not rater user";
}
