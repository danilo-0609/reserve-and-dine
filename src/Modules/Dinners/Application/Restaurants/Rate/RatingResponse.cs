namespace Dinners.Application.Restaurants.Rate;

public sealed record RatingResponse(Guid RestaurantRatingId,
    int Stars,
    string Comment,
    DateTime RatedAt,
    DateTime? UpdatedAt);
