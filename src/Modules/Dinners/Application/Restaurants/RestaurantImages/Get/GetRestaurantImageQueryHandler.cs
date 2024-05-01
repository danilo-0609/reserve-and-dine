using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Dinners.Domain.Restaurants.RestaurantInformations;
using ErrorOr;

namespace Dinners.Application.Restaurants.RestaurantImages.Get;

internal sealed class GetRestaurantImageQueryHandler : IQueryHandler<GetRestaurantImageQuery, ErrorOr<BlobObject>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IRestaurantBlobService _blobService;

    public GetRestaurantImageQueryHandler(IRestaurantRepository restaurantRepository, IRestaurantBlobService blobService)
    {
        _restaurantRepository = restaurantRepository;
        _blobService = blobService;
    }

    public async Task<ErrorOr<BlobObject>> Handle(GetRestaurantImageQuery request, CancellationToken cancellationToken)
    {
        var restaurantImageUrl = await _restaurantRepository.GetRestaurantImageUrlById(
            RestaurantId.Create(request.Id), 
            RestaurantImageUrlId.Create(request.imageId),
            cancellationToken);

        if (restaurantImageUrl is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        BlobObject? blob = await _blobService.GetBlobAsync(restaurantImageUrl);
        
        if (blob is null)
        {
            return RestaurantErrorCodes.ImageNotFound;
        }

        return blob;
    }
}
