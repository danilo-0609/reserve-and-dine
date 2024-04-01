using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.RestaurantRatings;
using Microsoft.EntityFrameworkCore;

namespace Dinners.Infrastructure.Domain.Restaurants.Ratings;

internal sealed class RatingRepository : IRestaurantRatingRepository
{
    private readonly DinnersDbContext _dbContext;

    public RatingRepository(DinnersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(RestaurantRating rating, CancellationToken cancellationToken)
    {
        await _dbContext.Ratings.AddAsync(rating, cancellationToken);
    }

    public async Task DeleteAsync(RestaurantRatingId restaurantRatingId, CancellationToken cancellationToken)
    {
        await _dbContext
            .Ratings
            .Where(r => r.Id ==  restaurantRatingId)
            .ExecuteDeleteAsync();
    }

    public async Task<RestaurantRating?> GetByIdAsync(RestaurantRatingId ratingId, CancellationToken cancellationToken)
    {
        return await _dbContext
            .Ratings
            .Where(r => r.Id == ratingId)
            .SingleOrDefaultAsync();
    }

    public async Task<List<RestaurantRating>> GetRatingsByRestaurantId(RestaurantId restaurantId, CancellationToken cancellationToken)
    {
        IReadOnlyList<RestaurantRatingId>? restaurantRatings = await _dbContext
            .Restaurants
            .Where(r => r.Id == restaurantId)
            .Select(r => r.RestaurantRatingIds)
            .SingleOrDefaultAsync();

        if (restaurantRatings is null)
        {
            return new List<RestaurantRating>();
        }

        var ratings = restaurantRatings.ToList().ConvertAll(async ratingId =>
        {
            var rating = await _dbContext.Ratings.Where(r => r.Id == ratingId).SingleAsync();

            return rating;
        });

        RestaurantRating[] ratingsResponse = await Task.WhenAll(ratings);

        return ratingsResponse.ToList();
    }

    public Task UpdateAsync(RestaurantRating rating, CancellationToken cancellationToken)
    {
        _dbContext.Ratings.Update(rating);

        return Task.CompletedTask;
    }
}
