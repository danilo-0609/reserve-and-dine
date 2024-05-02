using Dinners.Application.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Errors;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Menus.Delete;

internal sealed class DeleteMenuCommandHandler : ICommandHandler<DeleteMenuCommand, ErrorOr<Unit>>
{
    private readonly IMenuRepository _menuRepository;

    public DeleteMenuCommandHandler(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
    {
        var menuId = MenuId.Create(request.Id);

        bool menuExists = await _menuRepository.ExistsAsync(menuId, cancellationToken);

        if (menuExists is false)
        {
            return MenuErrorCodes.NotFound;
        }

        await _menuRepository.DeleteAsync(menuId);

        return Unit.Value;
    }
}
