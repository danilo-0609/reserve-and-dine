using Dinners.Application.Common;
using Dinners.Domain.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Errors;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Menus.MenuSpecification;

internal sealed class UpdateMenuDetailsCommandHandler : ICommandHandler<UpdateMenuDetailsCommand, ErrorOr<Unit>>
{
    private readonly IMenuRepository _menuRepository;

    public UpdateMenuDetailsCommandHandler(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(UpdateMenuDetailsCommand request, CancellationToken cancellationToken)
    {
        Menu? menu = await _menuRepository.GetByIdAsync(MenuId.Create(request.MenuId), cancellationToken);
    
        if (menu is null)
        {
            return MenuErrorCodes.NotFound;
        }

        var menuType = GetMenuType(request.MenuType);

        menu.UpdateMenuDetails(request.Title,
            request.Description,
            menuType,
            new Price(request.Money, request.Currency),
            request.Discount,
            menu.MenuDetails.MenuImagesUrl,
            request.Tags!,
            request.IsVegetarian,
            request.PrimaryChefName,
            request.HasAlcohol,
            request.DiscountTerms);
    
        await _menuRepository.UpdateAsync(menu, cancellationToken);

        return Unit.Value;
    }

    private MenuType GetMenuType(string menuType)
    {
        if (menuType == "Breakfast")
        {
            return MenuType.Breakfast;
        }

        if (menuType == "Lunch")
        {
            return MenuType.Lunch;
        }

        return MenuType.Dinner;
    }
}
