using Dinners.Application.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Errors;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Menus.MenuImages.Add;

internal sealed class AddMenuImageCommandHandler : ICommandHandler<AddMenuImageCommand, ErrorOr<Unit>>
{
    private readonly IMenuRepository _menuRepository;
    private readonly IMenuBlobService _blobService;

    public AddMenuImageCommandHandler(IMenuRepository menuRepository, IMenuBlobService blobService)
    {
        _menuRepository = menuRepository;
        _blobService = blobService;
    }

    public async Task<ErrorOr<Unit>> Handle(AddMenuImageCommand request, CancellationToken cancellationToken)
    {
        Menu? menu = await _menuRepository.GetByIdAsync(MenuId.Create(request.Id), cancellationToken);
    
        if (menu is null)
        {
            return MenuErrorCodes.NotFound;
        }

        string imageUrl = await _blobService.UploadFileBlobAsync(request.FilePath, request.FormFile.FileName);

        menu.MenuDetails.AddImage(imageUrl);

        await _menuRepository.UpdateAsync(menu, cancellationToken);

        return Unit.Value;
    }
}
