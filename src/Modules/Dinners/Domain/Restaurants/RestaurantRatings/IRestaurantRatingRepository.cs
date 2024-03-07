namespace Dinners.Domain.Restaurants.RestaurantRatings;

public interface IRestaurantRatingRepository
{
    Task AddAsync(RestaurantRating rating, CancellationToken cancellationToken);

    Task UpdateAsync(RestaurantRating rating, CancellationToken cancellationToken);

    Task<RestaurantRating?> GetByIdAsync(RestaurantRatingId ratingId, CancellationToken cancellationToken);

    Task<List<RestaurantRating>> GetRatingsByRestaurantId(RestaurantId restaurantId, CancellationToken cancellationToken);

    Task DeleteAsync(RestaurantRatingId restaurantRatingId, CancellationToken cancellationToken);
}
