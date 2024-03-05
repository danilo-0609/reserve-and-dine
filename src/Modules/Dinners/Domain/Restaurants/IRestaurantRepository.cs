using Dinners.Domain.Restaurants.RestaurantTables;
using Domain.Restaurants;

namespace Dinners.Domain.Restaurants;

public interface IRestaurantRepository
{
    Task<bool> ExistsAsync(RestaurantId restaurantId);

    Task<Restaurant?> GetRestaurantById(RestaurantId restaurantId);

    Task UpdateAsync(Restaurant restaurant);

    Task<List<RestaurantTable>> GetRestaurantTablesById(RestaurantId restaurantId, CancellationToken cancellationToken); 
}
