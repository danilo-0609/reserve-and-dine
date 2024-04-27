using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using ErrorOr;
using MediatR;
using Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Dinners.Domain.Restaurants.RestaurantInformations;

namespace Dinners.Application.Restaurants.RestaurantImages.Insert;

internal sealed class InsertRestaurantImagesCommandHandler : ICommandHandler<InsertRestaurantImagesCommand, ErrorOr<Unit>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IRestaurantBlobService _blobService;

    public InsertRestaurantImagesCommandHandler(IRestaurantRepository restaurantRepository, IRestaurantBlobService blobService)
    {
        _restaurantRepository = restaurantRepository;
        _blobService = blobService;
    }

    public async Task<ErrorOr<Unit>> Handle(InsertRestaurantImagesCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.Id));

        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        var restaurantImageUrlId = RestaurantImageUrlId.CreateUnique();

        string imageUrl = await _blobService.UploadFileBlobAsync(request.FilePath, $"{restaurantImageUrlId.Value}-{request.FormFile.FileName}");
        
        restaurant.AddImage(imageUrl, restaurantImageUrlId);

        await _restaurantRepository.UpdateAsync(restaurant);

        return Unit.Value;
    }
}
