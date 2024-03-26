using Dinners.Application.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Errors;
using ErrorOr;

namespace Dinners.Application.Menus.GetById;

internal sealed class GetMenuByIdQueryHandler : IQueryHandler<GetMenuByIdQuery, ErrorOr<MenuResponse>>
{
    private readonly IMenuRepository _menuRepository;

    public GetMenuByIdQueryHandler(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<ErrorOr<MenuResponse>> Handle(GetMenuByIdQuery request, CancellationToken cancellationToken)
    {
        Menu? menu = await _menuRepository.GetByIdAsync(MenuId.Create(request.MenuId), cancellationToken);

        if (menu is null)
        {
            return MenuErrorCodes.NotFound;
        }

        var menuDetailsResponse = new MenuDetailsResponse(menu.MenuDetails.Title,
            menu.MenuDetails.Description,
            menu.MenuDetails.MenuType.Value,
            menu.MenuDetails.Price,
            menu.MenuDetails.Discount,
            menu.MenuDetails.Tags.ConvertAll(tag => tag.Value)!,
            menu.MenuDetails.IsVegetarian,
            menu.MenuDetails.PrimaryChefName,
            menu.MenuDetails.HasAlcohol,
            menu.MenuDetails.DiscountTerms);

        var dishSpecificationResponse = new DishSpecificationResponse(
            menu.DishSpecification.Ingredients.ConvertAll(ingredient => ingredient.Value)!,
            menu.DishSpecification.MainCourse,
            menu.DishSpecification.SideDishes,
            menu.DishSpecification.Appetizers,
            menu.DishSpecification.Beverages,
            menu.DishSpecification.Desserts,
            menu.DishSpecification.Sauces,
            menu.DishSpecification.Condiments,
            menu.DishSpecification.Coffee);

        var menuScheduleResponse = new MenuScheduleResponse(menu.MenuSchedule.Days.ConvertAll(value => value.DayOfWeek),
            menu.MenuSchedule.AvailableMenuHours.Start,
            menu.MenuSchedule.AvailableMenuHours.End);

        var menuResponse = new MenuResponse(menu.Id.Value,
            menu.RestaurantId.Value,
            menuDetailsResponse,
            dishSpecificationResponse,
            menuScheduleResponse,
            menu.CreatedOn,
            menu.UpdatedOn);

        return menuResponse;
    }
}
