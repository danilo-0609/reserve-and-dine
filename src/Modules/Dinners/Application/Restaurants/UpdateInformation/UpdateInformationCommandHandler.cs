using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Domain.Restaurants;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.UpdateInformation;

internal sealed class UpdateInformationCommandHandler : ICommandHandler<UpdateInformationCommand, ErrorOr<Unit>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public UpdateInformationCommandHandler(IRestaurantRepository restaurantRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _restaurantRepository = restaurantRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<Unit>> Handle(UpdateInformationCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.RestaurantId));
    
        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        var updateInformation = restaurant.UpdateInformation(_executionContextAccessor.UserId,
            request.Title,
            request.Description,
            request.Type);
    
        if (updateInformation.IsError)
        {
            return updateInformation.FirstError;
        }

        await _restaurantRepository.UpdateAsync(restaurant);

        return Unit.Value;
    }
}
