using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Domain.Restaurants;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.UpdateContact;

internal sealed class UpdateRestaurantContactCommandHandler : ICommandHandler<UpdateRestaurantContactCommand, ErrorOr<Unit>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public UpdateRestaurantContactCommandHandler(IRestaurantRepository restaurantRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _restaurantRepository = restaurantRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<Unit>> Handle(UpdateRestaurantContactCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.RestaurantId));

        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        var updateContact = restaurant.UpdateContact(_executionContextAccessor.UserId, 
            request.Email,
            request.Whatsapp,
            request.Facebook,
            request.PhoneNumber,
            request.Instagram,
            request.Twitter,
            request.TikTok,
            request.Website);
    
        if (updateContact.IsError)
        {
            return updateContact.FirstError;
        }

        await _restaurantRepository.UpdateAsync(restaurant);

        return Unit.Value;
    }
}
