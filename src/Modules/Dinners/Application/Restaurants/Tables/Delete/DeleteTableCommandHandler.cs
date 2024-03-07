using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Domain.Restaurants;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.Tables.Delete;

internal sealed class DeleteTableCommandHandler : ICommandHandler<DeleteTableCommand, ErrorOr<Unit>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public DeleteTableCommandHandler(IRestaurantRepository restaurantRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _restaurantRepository = restaurantRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteTableCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.RestaurantId));

        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        ErrorOr<Unit> deleteTable = restaurant.DeleteTable(_executionContextAccessor.UserId, request.Number);
        
        if (deleteTable.IsError)
        {
            return deleteTable.FirstError;
        }

        await _restaurantRepository.UpdateAsync(restaurant);

        return Unit.Value;
    }
}
