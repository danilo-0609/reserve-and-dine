using Dinners.Domain.Reservations;
using Dinners.Domain.Restaurants;
using Dinners.Infrastructure.Resolvers;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Threading;

namespace Dinners.Infrastructure.Cache.Reservations;

internal sealed class CacheReservationRepository : IReservationRepository
{
    private readonly IReservationRepository _decorated;
    private readonly IDistributedCache _distributedCache;
    private readonly DinnersDbContext _dbContext;

    public CacheReservationRepository(IReservationRepository decorated, IDistributedCache distributedCache, DinnersDbContext dbContext)
    {
        _decorated = decorated;
        _distributedCache = distributedCache;
        _dbContext = dbContext;
    }

    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken)
    {
        await _decorated.AddAsync(reservation, cancellationToken);
    }

    public async Task DeleteAsync(ReservationId reservationId, CancellationToken cancellationToken)
    {
        await _decorated.DeleteAsync(reservationId, cancellationToken);
    }

    public async Task<Reservation?> GetByIdAsync(ReservationId reservationId, CancellationToken cancellationToken)
    {
        string key = $"reservation-{reservationId.Value}";
        
        string? cachedReservation = await _distributedCache.GetStringAsync(
            key, 
            cancellationToken);

        Reservation? reservation;
        if (string.IsNullOrEmpty(cachedReservation))
        {
            reservation = await _decorated.GetByIdAsync(reservationId, cancellationToken);
        
            if (reservation is null)
            {
                return reservation;
            }

            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(reservation,
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                }),
                cancellationToken);

            return reservation;
        }

        reservation = JsonConvert.DeserializeObject<Reservation>(cachedReservation,
            new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new PrivateResolver()
            });

        if (reservation is not null)
        {
            _dbContext.Set<Reservation>().Attach(reservation);
        }

        return reservation;
    }

    public async Task<Reservation?> GetByRestaurantIdAndReservationTimeAsync(RestaurantId restaurantId, DateTime reservationDateTime, CancellationToken cancellation)
    {
        string key = $"reservation-restaurantId-{restaurantId.Value}-dateTime-{reservationDateTime}";

        string? cachedReservation = await _distributedCache.GetStringAsync(
            key, 
            cancellation);

        Reservation? reservation;
        if (string.IsNullOrEmpty(cachedReservation))
        {
            reservation = await _decorated.GetByRestaurantIdAndReservationTimeAsync(restaurantId, reservationDateTime, cancellation);

            if (reservation is null)
            {
                return reservation;
            }

            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(reservation,
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                }),
                cancellation);

            return reservation;
        }

        reservation = JsonConvert.DeserializeObject<Reservation>(cachedReservation,
            new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new PrivateResolver()
            });

        if (reservation is not null)
        {
            _dbContext.Set<Reservation>().Attach(reservation);
        }

        return reservation;
    }

    public async Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken)
    {
        await _decorated.UpdateAsync(reservation, cancellationToken);
    }
}
