using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Domain.Restaurants;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.ChangeLocalization;

internal sealed class ChangeRestaurantLocalizationCommandHandler : ICommandHandler<ChangeRestaurantLocalizationCommand, ErrorOr<Unit>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public ChangeRestaurantLocalizationCommandHandler(IRestaurantRepository restaurantRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _restaurantRepository = restaurantRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<Unit>> Handle(ChangeRestaurantLocalizationCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.RestaurantId));

        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        var changeLocalization = restaurant.ChangeLocalization(_executionContextAccessor.UserId,
            request.Country,
            request.City,
            request.Region,
            request.Neighborhood,
            request.Address,
            request.LocalizationDetails);

        if (changeLocalization.IsError)
        {
            return changeLocalization.FirstError;
        }

        await _restaurantRepository.UpdateAsync(restaurant);

        return Unit.Value;
    }
}
