using Dinners.Application.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Details;
using Dinners.Domain.Menus.Errors;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Menus.MenuImages.Delete;

internal sealed class DeleteMenuImageCommandHandler : ICommandHandler<DeleteMenuImageCommand, ErrorOr<Success>>
{
    private readonly IMenuRepository _menuRepository;
    private readonly IMenuBlobService _blobService;

    public DeleteMenuImageCommandHandler(IMenuRepository menuRepository, IMenuBlobService blobService)
    {
        _menuRepository = menuRepository;
        _blobService = blobService;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteMenuImageCommand request, CancellationToken cancellationToken)
    {
        Menu? menu = await _menuRepository.GetByIdAsync(MenuId.Create(request.Id), cancellationToken);
    
        if (menu is null)
        {
            return MenuErrorCodes.NotFound;
        }

        var menuImageId = MenuImageUrlId.Create(request.ImageId);

        var menuImage = menu.MenuImagesUrl.Where(q => q.Id == menuImageId).SingleOrDefault();

        if (menuImage is null)
        {
            return Error.NotFound("Menu.ImageNotFound", "Image was not found");
        }

        await _blobService.DeleteBlobAsync(menuImage.Value);

        menu.DeleteImage(menuImage.Value, menuImage.Id);
    
        await _menuRepository.UpdateAsync(menu, cancellationToken);

        return new Success();
    }
}
