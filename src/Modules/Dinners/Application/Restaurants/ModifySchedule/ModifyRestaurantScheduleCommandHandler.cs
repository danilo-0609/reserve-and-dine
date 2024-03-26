using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Domain.Restaurants;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.ModifySchedule;

internal sealed class ModifyRestaurantScheduleCommandHandler : ICommandHandler<ModifyRestaurantScheduleCommand, ErrorOr<Unit>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public ModifyRestaurantScheduleCommandHandler(IRestaurantRepository restaurantRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _restaurantRepository = restaurantRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<Unit>> Handle(ModifyRestaurantScheduleCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.RestaurantId));

        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        var modifySchedule = restaurant.ModifySchedule(_executionContextAccessor.UserId, 
            request.Day, 
            request.Start, 
            request.End);
    
        if (modifySchedule.IsError)
        {
            return modifySchedule.FirstError;
        }

        await _restaurantRepository.UpdateAsync(restaurant);

        return Unit.Value;
    }
}
