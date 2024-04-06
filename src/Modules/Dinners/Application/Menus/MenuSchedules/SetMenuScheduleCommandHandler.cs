using Dinners.Application.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Errors;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Menus.MenuSchedules;

internal sealed class SetMenuScheduleCommandHandler : ICommandHandler<SetMenuScheduleCommand, ErrorOr<Unit>>
{
    private readonly IMenuRepository _menuRepository;

    public SetMenuScheduleCommandHandler(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(SetMenuScheduleCommand request, CancellationToken cancellationToken)
    {
        Menu? menu = await _menuRepository.GetByIdAsync(MenuId.Create(request.MenuId), cancellationToken);

        if (menu is null)
        {
            return MenuErrorCodes.NotFound;
        }

        menu.SetMenuSchedule(request.Day, request.Start, request.End);

        await _menuRepository.UpdateAsync(menu, cancellationToken);

        return Unit.Value;
    }
}
