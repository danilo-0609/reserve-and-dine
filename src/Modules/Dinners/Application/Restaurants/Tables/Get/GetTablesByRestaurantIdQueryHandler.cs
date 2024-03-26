using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Domain.Restaurants;
using ErrorOr;

namespace Dinners.Application.Restaurants.Tables.Get;

internal sealed class GetTablesByRestaurantIdQueryHandler : IQueryHandler<GetTablesByRestaurantIdQuery, ErrorOr<List<RestaurantTableResponse>>>
{
    private readonly IRestaurantRepository _restaurantRepository;

    public GetTablesByRestaurantIdQueryHandler(IRestaurantRepository restaurantRepository)
    {
        _restaurantRepository = restaurantRepository;
    }

    public async Task<ErrorOr<List<RestaurantTableResponse>>> Handle(GetTablesByRestaurantIdQuery request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.RestaurantId));

        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        return restaurant
            .RestaurantTables
            .ToList()
            .ConvertAll(table => 
                new RestaurantTableResponse(table.Number, 
                    table.Seats, 
                    table.IsPremium, 
                    table.IsOccupied, 
                    table.ReservedHours));
    }
}
