using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.RestaurantTables;
using Dinners.Infrastructure.Resolvers;
using Domain.Restaurants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Dinners.Infrastructure.Cache.Restaurants;

internal sealed class CacheRestaurantRepository : IRestaurantRepository
{
    private readonly IRestaurantRepository _decorated;
    private readonly IDistributedCache _distributedCache;
    private readonly DinnersDbContext _dbContext;

    public CacheRestaurantRepository(IRestaurantRepository decorated, IDistributedCache distributedCache, DinnersDbContext dbContext)
    {
        _decorated = decorated;
        _distributedCache = distributedCache;
        _dbContext = dbContext;
    }

    public async Task AddAsync(Restaurant restaurant, CancellationToken cancellationToken)
    {
        await _decorated.AddAsync(restaurant, cancellationToken);
    }

    public async Task DeleteAsync(RestaurantId restaurant, CancellationToken cancellationToken)
    {
        await _decorated.DeleteAsync(restaurant, cancellationToken);
    }

    public async Task<bool> ExistsAsync(RestaurantId restaurantId)
    {
        return await _decorated.ExistsAsync(restaurantId);
    }

    public async Task<List<Restaurant>> GetByLocalizationAsync(string country, string region, string city, string? neighborhood, CancellationToken cancellationToken)
    {
        string key = $"restaurants-localization-{country}-{region}-{city}";

        string? cachedRestaurants = await _distributedCache.GetStringAsync(key,
        cancellationToken);

        List<Restaurant> restaurants;
        if (string.IsNullOrEmpty(cachedRestaurants))
        {
            restaurants = await _decorated.GetByLocalizationAsync(country, region, city, neighborhood, cancellationToken);

            if (!restaurants.Any())
            {
                return restaurants;
            }

            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(restaurants, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                }),
                cancellationToken);

            return restaurants;
        }

        restaurants = JsonConvert.DeserializeObject<List<Restaurant>>(cachedRestaurants,
        new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new PrivateResolver()
        })!;

        if (restaurants.Any())
        {
            _dbContext.Set<Restaurant>().AttachRange(restaurants);
        }

        return restaurants;
    }

    public async Task<Restaurant?> GetRestaurantById(RestaurantId restaurantId)
    {
        string key = $"restaurant-{restaurantId.Value}";

        string? cachedRestaurant = await _distributedCache.GetStringAsync(key);

        Restaurant? restaurant;
        if (string.IsNullOrEmpty(cachedRestaurant))
        {
            restaurant = await _decorated.GetRestaurantById(restaurantId);

            if (restaurant is null)
            {
                return restaurant;
            }

            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(restaurant,
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                }));

            return restaurant;
        }

        restaurant = JsonConvert.DeserializeObject<Restaurant>(cachedRestaurant,
            new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new PrivateResolver()
            });

        if (restaurant is not null)
        {
            _dbContext.Set<Restaurant>().Attach(restaurant);
        }

        return restaurant;
    }

    public async Task<List<string>> GetRestaurantImagesUrlById(RestaurantId restaurantId, CancellationToken cancellationToken)
    {
        string key = $"restaurantImagesUrls-{restaurantId.Value}";

        string? cachedRestaurantImagesUrls = await _distributedCache.GetStringAsync(key,
            cancellationToken);

        List<string> restaurantImagesUrls;
        if (string.IsNullOrEmpty(cachedRestaurantImagesUrls))
        {
            restaurantImagesUrls = await _decorated.GetRestaurantImagesUrlById(restaurantId, cancellationToken);

            if (!restaurantImagesUrls.Any())
            {
                return restaurantImagesUrls;
            }

            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(restaurantImagesUrls, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                }),
                cancellationToken);

            return restaurantImagesUrls;
        }

        restaurantImagesUrls = JsonConvert.DeserializeObject<List<string>>(cachedRestaurantImagesUrls,
        new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new PrivateResolver()
        })!;

        if (!restaurantImagesUrls.Any())
        {
            _dbContext.Set<Restaurant>().Attach(
                 await _dbContext
                .Restaurants
                .Where(r => r.Id == restaurantId)
                .SingleAsync());
        }

        return restaurantImagesUrls;
    }

    public async Task<List<Restaurant>> GetRestaurantsByNameAsync(string name, CancellationToken cancellationToken)
    {
        string key = $"restaurants-{name}";

        string? cachedRestaurants = await _distributedCache.GetStringAsync(key,
        cancellationToken);

        List<Restaurant> restaurants;
        if (string.IsNullOrEmpty(cachedRestaurants))
        {
            restaurants = await _decorated.GetRestaurantsByNameAsync(name, cancellationToken);

            if (!restaurants.Any())
            {
                return restaurants;
            }

            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(restaurants, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                }),
                cancellationToken);

            return restaurants;
        }

        restaurants = JsonConvert.DeserializeObject<List<Restaurant>>(cachedRestaurants,
        new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new PrivateResolver()
        })!;

        if (restaurants.Any())
        {
            _dbContext.Set<Restaurant>().AttachRange(restaurants);
        }

        return restaurants;
    }

    public async Task<List<RestaurantTable>> GetRestaurantTablesById(RestaurantId restaurantId, CancellationToken cancellationToken)
    {
        string key = $"table-{restaurantId.Value}";

        var cachedTables = await _distributedCache.GetStringAsync(key, cancellationToken);

        List<RestaurantTable> restaurantTables;
        if (string.IsNullOrEmpty(cachedTables))
        {
            restaurantTables = await _decorated.GetRestaurantTablesById(restaurantId, cancellationToken);
        
            if (!restaurantTables.Any())
            {
                return restaurantTables;
            }

            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(restaurantTables, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                }),
                cancellationToken);

            return restaurantTables;
        }
        
        restaurantTables = JsonConvert.DeserializeObject<List<RestaurantTable>>(cachedTables,
        new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new PrivateResolver()
        })!;

        if (restaurantTables.Any())
        {
            _dbContext.Set<RestaurantTable>().AttachRange(restaurantTables);

            _dbContext.Set<Restaurant>().Attach(await _dbContext
                .Restaurants
                .Where(r => r.Id == restaurantId)
                .SingleAsync());
        }

        return restaurantTables;
    }

    public async Task UpdateAsync(Restaurant restaurant)
    {
        await _decorated.UpdateAsync(restaurant);
    }
}
