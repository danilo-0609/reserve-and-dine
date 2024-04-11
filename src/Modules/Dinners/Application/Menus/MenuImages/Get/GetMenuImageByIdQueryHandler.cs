using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Details;
using Dinners.Domain.Menus.Errors;
using ErrorOr;

namespace Dinners.Application.Menus.MenuImages.Get;

internal sealed class GetMenuImageByIdQueryHandler : IQueryHandler<GetMenuImageByIdQuery, ErrorOr<BlobObject>>
{
    private readonly IMenuRepository _menuRepository;
    private readonly IMenuBlobService _blobService;

    public GetMenuImageByIdQueryHandler(IMenuRepository menuRepository, IMenuBlobService blobService)
    {
        _menuRepository = menuRepository;
        _blobService = blobService;
    }

    public async Task<ErrorOr<BlobObject>> Handle(GetMenuImageByIdQuery request, CancellationToken cancellationToken)
    {
        string? menuImageUrl = await _menuRepository.GetMenuImageUrlById(MenuId.Create(request.MenuId), MenuImageUrlId.Create(request.ImageId), cancellationToken);

        if (menuImageUrl is null)
        {
            return MenuErrorCodes.NotFound;
        }

        var blobObject = await _blobService.GetBlobAsync(menuImageUrl);

        if (blobObject is null)
        {
            return MenuErrorCodes.ImagesNotFound;
        }

        return blobObject;
    }
}
