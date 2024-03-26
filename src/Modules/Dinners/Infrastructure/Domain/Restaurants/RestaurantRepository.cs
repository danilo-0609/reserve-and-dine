using Dinners.Domain.Menus;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.RestaurantTables;
using Domain.Restaurants;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

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

    public async Task DeleteAsync(RestaurantId restaurant, CancellationToken cancellationToken)
    {
        await _dbContext
            .Restaurants
            .Where(r => r.Id == restaurant)
            .ExecuteDeleteAsync();
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
        .SelectMany(x => x.RestaurantInformation.RestaurantImagesUrl
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

    public async Task UpdateAsync(Restaurant restaurant)
    {
        await _dbContext
           .Restaurants
           .ExecuteUpdateAsync(x =>
               x.SetProperty(r => r.Id, restaurant.Id)
                .SetProperty(r => r.NumberOfTables, restaurant.NumberOfTables)
                .SetProperty(r => r.AvailableTablesStatus, restaurant.AvailableTablesStatus)
                .SetProperty(r => r.RestaurantInformation, restaurant.RestaurantInformation)
                .SetProperty(r => r.RestaurantLocalization, restaurant.RestaurantLocalization)
                .SetProperty(r => r.RestaurantScheduleStatus, restaurant.RestaurantScheduleStatus)
                .SetProperty(r => r.RestaurantSchedules, restaurant.RestaurantSchedules)
                .SetProperty(r => r.RestaurantContact, restaurant.RestaurantContact)
                .SetProperty(r => r.RestaurantRatingIds, restaurant.RestaurantRatingIds)
                .SetProperty(r => r.RestaurantClients, restaurant.RestaurantClients)
                .SetProperty(r => r.RestaurantTables, restaurant.RestaurantTables)
                .SetProperty(r => r.RestaurantAdministrations, restaurant.RestaurantAdministrations)
                .SetProperty(r => r.PostedAt, restaurant.PostedAt));
    }
}
