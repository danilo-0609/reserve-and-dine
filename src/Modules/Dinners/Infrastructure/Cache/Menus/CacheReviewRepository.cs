using Dinners.Domain.Menus.MenuReviews;
using Dinners.Domain.Reservations.ReservationsPayments;
using Dinners.Infrastructure.Resolvers;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Dinners.Infrastructure.Cache.Menus;

internal sealed class CacheReviewRepository : IReviewRepository
{
    private readonly IReviewRepository _decorated;
    private readonly IDistributedCache _distributedCache;
    private readonly DinnersDbContext _dbContext;

    public CacheReviewRepository(IReviewRepository decorated, IDistributedCache distributedCache, DinnersDbContext dbContext)
    {
        _decorated = decorated;
        _distributedCache = distributedCache;
        _dbContext = dbContext;
    }

    public async Task AddAsync(MenuReview menuReview)
    {
        await _decorated.AddAsync(menuReview);
    }

    public async Task<MenuReview?> GetByIdAsync(MenuReviewId menuReviewId, CancellationToken cancellationToken)
    {
        string key = $"review-{menuReviewId.Value}";

        string? cachedReview = await _distributedCache.GetStringAsync(key, cancellationToken);

        MenuReview? review;
        if (string.IsNullOrEmpty(cachedReview))
        {
            review = await _decorated.GetByIdAsync(menuReviewId, cancellationToken);

            if (review is null)
            {
                return review;
            }

            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(review,
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                }),
                cancellationToken);

            return review;
        }

        review = JsonConvert.DeserializeObject<MenuReview>(cachedReview,
            new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new PrivateResolver()
            });

        if (review is not null)
        {
            _dbContext.Set<MenuReview>().Attach(review);
        }

        return review;
    }

    public async Task UpdateAsync(MenuReview menuReview)
    {
        await _decorated.UpdateAsync(menuReview);
    }
}
