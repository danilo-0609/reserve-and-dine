using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Domain.Restaurants;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.Administration.Delete;

internal sealed class DeleteAdministrationCommandHandler : ICommandHandler<DeleteAdministrationCommand, ErrorOr<Unit>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public DeleteAdministrationCommandHandler(IRestaurantRepository restaurantRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _restaurantRepository = restaurantRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteAdministrationCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.RestaurantId));

        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        ErrorOr<Unit> deleteAdministrator = restaurant.DeleteAdministrator(request.AdministratorId, _executionContextAccessor.UserId);
        
        if (deleteAdministrator.IsError)
        {
            return deleteAdministrator.FirstError;
        }

        await _restaurantRepository.UpdateAsync(restaurant);

        return Unit.Value;
    }
}
