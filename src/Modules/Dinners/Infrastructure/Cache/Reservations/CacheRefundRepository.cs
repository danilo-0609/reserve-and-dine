using Dinners.Domain.Reservations.Refunds;
using Dinners.Domain.Reservations.ReservationsPayments;
using Dinners.Infrastructure.Resolvers;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Dinners.Infrastructure.Cache.Reservations;

internal sealed class CacheRefundRepository : IRefundRepository
{
    private readonly IRefundRepository _decorated;
    private readonly IDistributedCache _distributedCache;
    private readonly DinnersDbContext _dbContext;

    public CacheRefundRepository(IRefundRepository decorated, IDistributedCache distributedCache, DinnersDbContext dbContext)
    {
        _decorated = decorated;
        _distributedCache = distributedCache;
        _dbContext = dbContext;
    }

    public async Task AddAsync(Refund refund)
    {
        await _decorated.AddAsync(refund);
    }

    public async Task<Refund?> GetByIdAsync(RefundId refundId, CancellationToken cancellationToken)
    {
        string key = $"refund-{refundId.Value}";

        string? cachedRefund = await _distributedCache.GetStringAsync(key, cancellationToken);

        Refund? refund;
        if (string.IsNullOrEmpty(cachedRefund))
        {
            refund = await _decorated.GetByIdAsync(refundId, cancellationToken);

            if (refund is null)
            {
                return refund;
            }

            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(refund,
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                }),
                cancellationToken);

            return refund;
        }

        refund = JsonConvert.DeserializeObject<Refund>(cachedRefund,
            new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new PrivateResolver()
            });

        if (refund is not null)
        {
            _dbContext.Set<Refund>().Attach(refund);
        }

        return refund;
    }
}
