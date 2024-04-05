using Dinners.Domain.Reservations.Refunds;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.RestaurantRatings;
using Dinners.Infrastructure.Domain.Restaurants.RestaurantsRatings;
using Dinners.Infrastructure.Resolvers;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Dinners.Infrastructure.Cache.Restaurants;

internal sealed class CacheRatingRepository : IRestaurantRatingRepository
{
    private readonly IRestaurantRatingRepository _decorated;
    private readonly IDistributedCache _distributedCache;
    private readonly DinnersDbContext _dbContext;

    public CacheRatingRepository(IRestaurantRatingRepository decorated, IDistributedCache distributedCache, DinnersDbContext dbContext)
    {
        _decorated = decorated;
        _distributedCache = distributedCache;
        _dbContext = dbContext;
    }

    public async Task AddAsync(RestaurantRating rating, CancellationToken cancellationToken)
    {
        await _decorated.AddAsync(rating, cancellationToken);
    }

    public async Task DeleteAsync(RestaurantRatingId restaurantRatingId, CancellationToken cancellationToken)
    {
        await _decorated.DeleteAsync(restaurantRatingId, cancellationToken);
    }

    public async Task<RestaurantRating?> GetByIdAsync(RestaurantRatingId ratingId, CancellationToken cancellationToken)
    {
        string key = $"rating-{ratingId.Value}";

        string? cachedRating = await _distributedCache.GetStringAsync(key, cancellationToken);

        RestaurantRating? rating;
        if (string.IsNullOrEmpty(cachedRating))
        {
            rating = await _decorated.GetByIdAsync(ratingId, cancellationToken);

            if (rating is null)
            {
                return rating;
            }

            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(rating,
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                }),
                cancellationToken);

            return rating;
        }

        rating = JsonConvert.DeserializeObject<RestaurantRating>(cachedRating,
            new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new PrivateResolver()
            });

        if (rating is not null)
        {
            _dbContext.Set<RestaurantRating>().Attach(rating);
        }

        return rating;
    }

    public async Task<List<RestaurantRating>> GetRatingsByRestaurantId(RestaurantId restaurantId, CancellationToken cancellationToken)
    {
        string key = $"ratings-{restaurantId.Value}";

        string? cachedRatings = await _distributedCache.GetStringAsync(key, cancellationToken);

        List<RestaurantRating> ratings;
        if (string.IsNullOrEmpty(cachedRatings))
        {
            ratings = await _decorated.GetRatingsByRestaurantId(restaurantId, cancellationToken);

            if (!ratings.Any())
            {
                return ratings;
            }

            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(ratings,
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                }),
                cancellationToken);

            return ratings;
        }

        ratings = JsonConvert.DeserializeObject<List<RestaurantRating>>(cachedRatings,
            new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new PrivateResolver()
            })!;

        if (!ratings.Any())
        {
            _dbContext.Set<RestaurantRating>().AttachRange(ratings);
        }

        return ratings;
    }

    public async Task UpdateAsync(RestaurantRating rating, CancellationToken cancellationToken)
    {
        await _decorated.UpdateAsync(rating, cancellationToken);
    }
}
