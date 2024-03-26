using Dinners.Application.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Errors;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Menus.MenuImages.Delete;

internal sealed class DeleteMenuImageCommandHandler : ICommandHandler<DeleteMenuImageCommand, ErrorOr<Unit>>
{
    private readonly IMenuRepository _menuRepository;
    private readonly IMenuBlobService _blobService;

    public DeleteMenuImageCommandHandler(IMenuRepository menuRepository, IMenuBlobService blobService)
    {
        _menuRepository = menuRepository;
        _blobService = blobService;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteMenuImageCommand request, CancellationToken cancellationToken)
    {
        Menu? menu = await _menuRepository.GetByIdAsync(MenuId.Create(request.Id), cancellationToken);
    
        if (menu is null)
        {
            return MenuErrorCodes.NotFound;
        }

        if (!menu.MenuDetails.MenuImagesUrl.Any(r => r.Value == request.MenuImageUrl))
        {
            return Error.NotFound("Menu.ImageNotFound", "Image was not found");
        }

        await _blobService.DeleteBlobAsync(request.MenuImageUrl);

        menu.MenuDetails.DeleteImage(request.MenuImageUrl);
    
        await _menuRepository.UpdateAsync(menu, cancellationToken);

        return Unit.Value;
    }
}
