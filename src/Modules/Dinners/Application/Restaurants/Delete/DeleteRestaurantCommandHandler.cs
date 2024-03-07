using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Domain.Restaurants;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.Delete;

internal sealed class DeleteRestaurantCommandHandler : ICommandHandler<DeleteRestaurantCommand, ErrorOr<Unit>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public DeleteRestaurantCommandHandler(IRestaurantRepository restaurantRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _restaurantRepository = restaurantRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteRestaurantCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.RestaurantId));
    
        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        if (!restaurant
            .RestaurantAdministrations
            .Any(q => q.AdministratorId == _executionContextAccessor.UserId))
        {
            return RestaurantErrorCodes.CannotDeleteRestaurant;
        }

        await _restaurantRepository.DeleteAsync(restaurant.Id, cancellationToken);

        return Unit.Value;
    }
}
