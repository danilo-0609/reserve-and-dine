using Dinners.Domain.Reservations.ReservationsPayments;
using Dinners.Infrastructure.Resolvers;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Dinners.Infrastructure.Cache.Reservations;

internal sealed class CachePaymentRepository : IReservationPaymentRepository
{
    private readonly IReservationPaymentRepository _decorated;
    private readonly IDistributedCache _distributedCache;
    private readonly DinnersDbContext _dbContext;

    public CachePaymentRepository(IReservationPaymentRepository decorated, IDistributedCache distributedCache, DinnersDbContext dbContext)
    {
        _decorated = decorated;
        _distributedCache = distributedCache;
        _dbContext = dbContext;
    }

    public async Task AddAsync(ReservationPayment payment, CancellationToken cancellationToken)
    {
        await _decorated.AddAsync(payment, cancellationToken);
    }

    public async Task<ReservationPayment?> GetByIdAsync(ReservationPaymentId paymentId, CancellationToken cancellationToken)
    {
        string key = $"payment-{paymentId.Value}";

        string? cachedPayment = await _distributedCache.GetStringAsync(key, cancellationToken);

        ReservationPayment? payment;
        if (string.IsNullOrEmpty(cachedPayment))
        {
            payment = await _decorated.GetByIdAsync(paymentId, cancellationToken);

            if (payment is null)
            {
                return payment;
            }

            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(payment,
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                }),
                cancellationToken);

            return payment;
        }

        payment = JsonConvert.DeserializeObject<ReservationPayment>(cachedPayment,
            new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new PrivateResolver()
            });

        if (payment is not null)
        {
            _dbContext.Set<ReservationPayment>().Attach(payment);
        }

        return payment;
    }
}
