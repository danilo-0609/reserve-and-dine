using BuildingBlocks.Domain.Entities;
using Dinners.Domain.Restaurants.RestaurantRatings.Events;
using Dinners.Domain.Restaurants.RestaurantRatings.Rules;
using ErrorOr;

namespace Dinners.Domain.Restaurants.RestaurantRatings;

public sealed class RestaurantRating : Entity<RestaurantRatingId, Guid>
{
    public new RestaurantRatingId Id { get; private set; }

    public Guid ClientId { get; private set; }

    public int Stars { get; private set; }

    public string Comment { get; private set; }

    public DateTime RatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    private RestaurantRating(RestaurantRatingId id,
        Guid clientId,
        int stars,
        string comment,
        DateTime ratedAt,
        DateTime? updatedAt = null)
    {
        Id = id;
        ClientId = clientId;
        Stars = stars;
        Comment = comment;
        RatedAt = ratedAt;
        UpdatedAt = updatedAt;
    }



    public static ErrorOr<RestaurantRating> GiveRating(RestaurantId restaurantId,
        string restaurantTitle,
        int stars,
        Guid clientId,
        bool hasVisited,
        DateTime ratedAt,
        string comment = "")
    {
        var rating = new RestaurantRating(RestaurantRatingId.CreateUnique(),
            clientId,
            stars,
            comment,
            ratedAt);

        var canRateRule = rating.CheckRule(new RatingCannotBeMadeWhenClientHasNotVisitedTheRestaurantRule(hasVisited));

        if (canRateRule.IsError)
        {
            return canRateRule.FirstError;
        }

        rating.AddDomainEvent(new RatingPublishedDomainEvent(Guid.NewGuid(),
            rating.Id,
            clientId,
            stars,
            restaurantId,
            restaurantTitle,
            DateTime.UtcNow));

        return rating;
    }

    public ErrorOr<RestaurantRating> Update(RestaurantRatingId id,
        int stars,
        Guid clientId,
        DateTime ratedAt,
        DateTime updatedAt,
        string comment = "")
    {
        var canUpdateRate = CheckRule(new CannotUpdateRatingWhenIsNotRaterUserRule(clientId, ClientId));

        if (canUpdateRate.IsError)
        {
            return canUpdateRate.FirstError;
        }

        return new RestaurantRating(
            id,
            clientId,
            stars,
            comment,
            ratedAt,
            updatedAt);
    }

    private RestaurantRating() { }
}

