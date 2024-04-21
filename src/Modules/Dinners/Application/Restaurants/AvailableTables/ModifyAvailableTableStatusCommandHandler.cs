using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Dinners.Domain.Restaurants.RestaurantTables;
using Domain.Restaurants;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.AvailableTables;

internal sealed class ModifyAvailableTableStatusCommandHandler : ICommandHandler<ModifyAvailableTableStatusCommand, ErrorOr<Unit>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public ModifyAvailableTableStatusCommandHandler(IRestaurantRepository restaurantRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _restaurantRepository = restaurantRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<Unit>> Handle(ModifyAvailableTableStatusCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.RestaurantId));

        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        var availableTablesStatus = restaurant.ModifyAvailableTablesStatus(
            _executionContextAccessor.UserId,
            GetAvailableTableStatus(request.AvailableTableStatus));

        if (availableTablesStatus.IsError)
        {
            return availableTablesStatus.FirstError;
        }

        await _restaurantRepository.UpdateAsync(restaurant);

        return Unit.Value;
    }

    private AvailableTablesStatus GetAvailableTableStatus(string status)
    {
        if (status == AvailableTablesStatus.NoAvailables.Value)
        {
            return AvailableTablesStatus.NoAvailables;
        }

        if (status == AvailableTablesStatus.Few.Value)
        {
            return AvailableTablesStatus.Few;
        }

        return AvailableTablesStatus.Availables;

    }
}
