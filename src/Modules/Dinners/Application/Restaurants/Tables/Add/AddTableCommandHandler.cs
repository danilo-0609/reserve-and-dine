using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Domain.Restaurants;
using ErrorOr;

namespace Dinners.Application.Restaurants.Tables.Add;

internal sealed class AddTableCommandHandler : ICommandHandler<AddTableCommand, ErrorOr<Guid>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public AddTableCommandHandler(IRestaurantRepository restaurantRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _restaurantRepository = restaurantRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<Guid>> Handle(AddTableCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.RestaurantId));

        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        var table = restaurant.AddTable(_executionContextAccessor.UserId, 
            request.Number, 
            request.Seats, 
            request.IsPremium);
    
        if (table.IsError)
        {
            return table.FirstError;
        }

        await _restaurantRepository.UpdateAsync(restaurant);

        return table.Value.Value;
    }
}
