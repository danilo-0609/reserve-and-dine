using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using ErrorOr;
using MediatR;
using Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Dinners.Domain.Restaurants.RestaurantInformations;

namespace Dinners.Application.Restaurants.RestaurantImages.Remove;

internal sealed class RemoveRestaurantImageCommandHandler : ICommandHandler<RemoveRestaurantImageCommand, ErrorOr<Unit>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IRestaurantBlobService _blobService;

    public RemoveRestaurantImageCommandHandler(IRestaurantRepository restaurantRepository, IRestaurantBlobService blobService)
    {
        _restaurantRepository = restaurantRepository;
        _blobService = blobService;
    }

    public async Task<ErrorOr<Unit>> Handle(RemoveRestaurantImageCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.Id));

        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        var restaurantImageUrlId = RestaurantImageUrlId.Create(request.imageId);

        if (!restaurant.RestaurantImagesUrl.Any(r => r.Id == restaurantImageUrlId))
        {
            return Error.NotFound("Restaurant.ImageNotFound", "Restaurant was not found");
        }

        var restaurantImageUrl = restaurant.RestaurantImagesUrl
            .Where(r => r.Id == restaurantImageUrlId)
            .Single();

        await _blobService.DeleteBlobAsync(restaurantImageUrl.Value);

        restaurant.RemoveImage(restaurantImageUrl.Value, restaurantImageUrl.Id);

        await _restaurantRepository.UpdateAsync(restaurant);

        return Unit.Value;
    }
}
