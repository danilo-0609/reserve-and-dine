using Dinners.Application.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Errors;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Menus.DishSpecifications;

internal sealed class UpdateDishSpecificationCommandHandler : ICommandHandler<UpdateDishSpecificationCommand, ErrorOr<Unit>>
{
    private readonly IMenuRepository _menuRepository;

    public UpdateDishSpecificationCommandHandler(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(UpdateDishSpecificationCommand request, CancellationToken cancellationToken)
    {
        Menu? menu = await _menuRepository.GetByIdAsync(MenuId.Create(request.MenuId), cancellationToken);

        if (menu is null)
        {
            return MenuErrorCodes.NotFound;
        }

        menu.UpdateDishSpecification(request.MainCourse,
            request.SideDishes,
            request.Appetizers,
            request.Beverages,
            request.Desserts,
            request.Sauces,
            request.Condiments,
            request.Coffee);

        await _menuRepository.UpdateAsync(menu, cancellationToken);

        return Unit.Value;
    }
}
