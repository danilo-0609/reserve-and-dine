using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Errors;
using ErrorOr;

namespace Dinners.Application.Menus.MenuImages.Get;

internal sealed class GetMenuImagesByIdQueryHandler : IQueryHandler<GetMenuImagesByIdQuery, ErrorOr<List<BlobObject>>>
{
    private readonly IMenuRepository _menuRepository;
    private readonly IMenuBlobService _blobService;

    public GetMenuImagesByIdQueryHandler(IMenuRepository menuRepository, IMenuBlobService blobService)
    {
        _menuRepository = menuRepository;
        _blobService = blobService;
    }

    public async Task<ErrorOr<List<BlobObject>>> Handle(GetMenuImagesByIdQuery request, CancellationToken cancellationToken)
    {
        List<string> menuImagesUrl = await _menuRepository.GetMenuImagesUrlById(MenuId.Create(request.MenuId), cancellationToken);

        if (!menuImagesUrl.Any())
        {
            return MenuErrorCodes.NotFound;
        }

        List<Task<BlobObject?>> blobObjects = menuImagesUrl.ConvertAll(async imageUrl =>
        {
            var blobObject = await _blobService.GetBlobAsync(imageUrl);

            return blobObject;
        });

        var blobs = await Task.WhenAll(blobObjects);

        if (!blobs.Any() && blobs is null)
        {
            return MenuErrorCodes.ImagesNotFound;
        }

        return blobs.ToList()!;
    }
}
