using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Domain.Restaurants;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.Open;

internal sealed class OpenRestaurantCommandHandler : ICommandHandler<OpenRestaurantCommand, ErrorOr<Guid>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public OpenRestaurantCommandHandler(IRestaurantRepository restaurantRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _restaurantRepository = restaurantRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<Guid>> Handle(OpenRestaurantCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.RestaurantId));

        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        ErrorOr<Unit> openRestaurant = restaurant.Open(_executionContextAccessor.UserId);

        if (openRestaurant.IsError)
        {
            return openRestaurant.FirstError;
        }

        await _restaurantRepository.UpdateAsync(restaurant);

        return restaurant.Id.Value;
    }
}
