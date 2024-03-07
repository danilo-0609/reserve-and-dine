using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Dinners.Domain.Restaurants.RestaurantUsers;
using Domain.Restaurants;
using ErrorOr;

namespace Dinners.Application.Restaurants.Administration.Add;

internal sealed class AddAdministrationCommandHandler : ICommandHandler<AddAdministrationCommand, ErrorOr<Guid>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public AddAdministrationCommandHandler(IRestaurantRepository restaurantRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _restaurantRepository = restaurantRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<Guid>> Handle(AddAdministrationCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.RestaurantId));
        
        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        ErrorOr<RestaurantAdministration> administrator = restaurant.AddAdministration(request.Name,
            request.NewAdministratorId, 
            request.AdministratorTitle,
            _executionContextAccessor.UserId);
        
        if (administrator.IsError)
        {
            return administrator.FirstError;
        }

        await _restaurantRepository.UpdateAsync(restaurant);

        return administrator.Value.AdministratorId;
    }
}
