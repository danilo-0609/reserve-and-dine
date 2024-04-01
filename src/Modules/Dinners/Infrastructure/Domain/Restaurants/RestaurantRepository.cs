using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.RestaurantTables;
using Domain.Restaurants;
using Microsoft.EntityFrameworkCore;

namespace Dinners.Infrastructure.Domain.Restaurants;

internal sealed class RestaurantRepository : IRestaurantRepository
{
    private readonly DinnersDbContext _dbContext;

    public RestaurantRepository(DinnersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Restaurant restaurant, CancellationToken cancellationToken)
    {
        await _dbContext.Restaurants.AddAsync(restaurant, cancellationToken);
    }

    public Task DeleteAsync(RestaurantId restaurant, CancellationToken cancellationToken)
    {
         _dbContext.Restaurants.Where(r => r.Id == restaurant).ExecuteDelete();

        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(RestaurantId restaurantId)
    {
        return await _dbContext
            .Restaurants
            .AnyAsync(r => r.Id == restaurantId);
    }

    public async Task<List<Restaurant>> GetByLocalizationAsync(string country, string region, string city, string? neighborhood, CancellationToken cancellationToken)
    {
        if (neighborhood is not null)
        {
          return await _dbContext
          .Restaurants
          .Where(r => r.RestaurantLocalization.Country == country &&
            r.RestaurantLocalization.Region == region &&
            r.RestaurantLocalization.City == city &&
            r.RestaurantLocalization.Neighborhood == neighborhood)
          .ToListAsync();
        }

        return await _dbContext
          .Restaurants
          .Where(r => r.RestaurantLocalization.Country == country &&
            r.RestaurantLocalization.Region == region &&
            r.RestaurantLocalization.City == city)
          .ToListAsync();
    }

    public async Task<Restaurant?> GetRestaurantById(RestaurantId restaurantId)
    {
        return await _dbContext
          .Restaurants
          .Where(r => r.Id == restaurantId)
          .SingleOrDefaultAsync();
    }

    public async Task<List<string>> GetRestaurantImagesUrlById(RestaurantId restaurantId, CancellationToken cancellationToken)
    {
        return await _dbContext
        .Restaurants
        .Where(r => r.Id == restaurantId)
        .SelectMany(x => x.RestaurantImagesUrl
            .Select(r => r.Value))
        .ToListAsync(cancellationToken);
    }

    public async Task<List<Restaurant>> GetRestaurantsByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext
            .Restaurants
            .Where(r => r.RestaurantInformation.Title == name)
            .ToListAsync();
    }

    public async Task<List<RestaurantTable>> GetRestaurantTablesById(RestaurantId restaurantId, CancellationToken cancellationToken)
    {
        IReadOnlyList<RestaurantTable>? tables = await _dbContext
            .Restaurants
            .Where(r => r.Id == restaurantId)
            .Select(t => t.RestaurantTables)
            .SingleOrDefaultAsync();

        return tables!.ToList();
    }

    public Task UpdateAsync(Restaurant restaurant)
    {
        _dbContext.Restaurants.Update(restaurant);

        return Task.CompletedTask;
    }
}
