using BuildingBlocks.Domain.Entities;
using Dinners.Domain.Restaurants.RestaurantRatings.Rules;
using ErrorOr;

namespace Dinners.Domain.Restaurants.RestaurantRatings;

public sealed class RestaurantRating : Entity<RestaurantRatingId, Guid>
{
    public new RestaurantRatingId Id { get; private set; }

    public RestaurantId RestaurantId { get; private set; }

    public Guid ClientId { get; private set;  }
    
    public int Stars { get; private set; }
    
    public string Comment { get; private set; }

    public DateTime RatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public RestaurantRating(RestaurantRatingId id,
        RestaurantId restaurantId,
        Guid clientId,
        int stars,
        string comment,
        DateTime ratedAt,
        DateTime? updatedAt = null)
    {
        Id = id;
        RestaurantId = restaurantId;
        ClientId = clientId;
        Stars = stars;
        Comment = comment;
        RatedAt = ratedAt;
        UpdatedAt = updatedAt;
    }

    public static ErrorOr<RestaurantRating> GiveRating(RestaurantId restaurantId,
        int stars,
        Guid clientId,
        bool hasVisited,
        DateTime ratedAt,
        string comment = "")
    {
        var rating = new RestaurantRating(RestaurantRatingId.CreateUnique(),
            restaurantId,
            clientId,
            stars,
            comment,
            ratedAt);

        var canRateRule = rating.CheckRule(new RatingCannotBeMadeWhenClientHasNotVisitedTheRestaurantRule(hasVisited));

        if (canRateRule.IsError)
        {
            return canRateRule.FirstError;
        }

        return rating;
    }

    public RestaurantRating Update(RestaurantRatingId id,
        RestaurantId restaurantId,
        int stars,
        Guid clientId,
        DateTime ratedAt,
        DateTime updatedAt,
        string comment = "")
    {
        return new RestaurantRating(
            id,
            restaurantId,
            clientId,
            stars,
            comment,
            ratedAt,
            updatedAt);

    }
}

