using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using ErrorOr;

namespace Dinners.Application.Restaurants.RestaurantImages.Get;

internal sealed class GetRestaurantImagesQueryHandler : IQueryHandler<GetRestaurantImagesQuery, ErrorOr<List<BlobObject>>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IRestaurantBlobService _blobService;

    public GetRestaurantImagesQueryHandler(IRestaurantRepository restaurantRepository, IRestaurantBlobService blobService)
    {
        _restaurantRepository = restaurantRepository;
        _blobService = blobService;
    }

    public async Task<ErrorOr<List<BlobObject>>> Handle(GetRestaurantImagesQuery request, CancellationToken cancellationToken)
    {
        List<string> restaurantImagesUrl = await _restaurantRepository.GetRestaurantImagesUrlById(RestaurantId.Create(request.Id), cancellationToken);

        if (!restaurantImagesUrl.Any())
        {
            return RestaurantErrorCodes.NotFound;
        }

        List<Task<BlobObject?>> blobObjects = restaurantImagesUrl.ConvertAll(async imageUrl =>
        {
            var blobObject = await _blobService.GetBlobAsync(imageUrl);

            return blobObject;
        });

        var blobs = await Task.WhenAll(blobObjects);

        if (!blobs.Any() && blobs is null)
        {
            return RestaurantErrorCodes.ImagesNotFound;
        }

        return blobs.ToList()!;
    }
}
