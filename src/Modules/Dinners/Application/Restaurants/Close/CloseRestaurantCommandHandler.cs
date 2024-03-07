using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Dinners.Domain.Restaurants.RestaurantSchedules;
using Domain.Restaurants;
using ErrorOr;

namespace Dinners.Application.Restaurants.Close;

internal sealed class CloseRestaurantCommandHandler : ICommandHandler<CloseRestaurantCommand, ErrorOr<Guid>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public CloseRestaurantCommandHandler(IRestaurantRepository restaurantRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _restaurantRepository = restaurantRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<Guid>> Handle(CloseRestaurantCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.RestaurantId));

        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        ErrorOr<RestaurantScheduleStatus> closeRestaurant = restaurant.Close(_executionContextAccessor.UserId);

        if (closeRestaurant.IsError)
        {
            return closeRestaurant.FirstError;
        }

        await _restaurantRepository.UpdateAsync(restaurant);

        return restaurant.Id.Value;
    }
}
