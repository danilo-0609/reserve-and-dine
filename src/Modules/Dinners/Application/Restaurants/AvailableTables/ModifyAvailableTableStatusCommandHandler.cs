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

    public ModifyAvailableTableStatusCommandHandler(IRestaurantRepository restaurantRepository)
    {
        _restaurantRepository = restaurantRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(ModifyAvailableTableStatusCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.RestaurantId));

        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        ErrorOr<AvailableTablesStatus> availableTablesStatus = restaurant.ModifyAvailableTableStatus(
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
