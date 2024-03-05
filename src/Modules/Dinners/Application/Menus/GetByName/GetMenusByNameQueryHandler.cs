using Dinners.Application.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Errors;
using ErrorOr;

namespace Dinners.Application.Menus.GetByName;

internal sealed class GetMenusByNameQueryHandler : IQueryHandler<GetMenusByNameQuery, ErrorOr<List<MenuResponse>>>
{
    private readonly IMenuRepository _menuRepository;

    public GetMenusByNameQueryHandler(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<ErrorOr<List<MenuResponse>>> Handle(GetMenusByNameQuery request, CancellationToken cancellationToken)
    {
        List<Menu> menus = await _menuRepository.GetMenusByNameAsync(request.Name, cancellationToken);

        if (!menus.Any())
        {
            return MenuErrorCodes.NotFound;
        }

        List<MenuResponse> menuResponses = menus.ConvertAll(menu =>
        {
            var menuDetailsResponse = new MenuDetailsResponse(menu.MenuDetails.Title,
            menu.MenuDetails.Description,
            menu.MenuDetails.MenuType.Value,
            menu.MenuDetails.Price,
            menu.MenuDetails.Discount,
            menu.MenuDetails.Tags!,
            menu.MenuDetails.IsVegetarian,
            menu.MenuDetails.PrimaryChefName,
            menu.MenuDetails.HasAlcohol,
            menu.MenuDetails.DiscountTerms);

            var dishSpecificationResponse = new DishSpecificationResponse(
                menu.DishSpecification.Ingredients!,
                menu.DishSpecification.MainCourse,
                menu.DishSpecification.SideDishes,
                menu.DishSpecification.Appetizers,
                menu.DishSpecification.Beverages,
                menu.DishSpecification.Desserts,
                menu.DishSpecification.Sauces,
                menu.DishSpecification.Condiments,
                menu.DishSpecification.Coffee);

            var menuScheduleResponse = new MenuScheduleResponse(menu.MenuSchedule.Days,
                menu.MenuSchedule.AvailableMenuHours.Start,
                menu.MenuSchedule.AvailableMenuHours.End);

            return new MenuResponse(menu.Id.Value,
                menu.RestaurantId.Value,
                menuDetailsResponse,
                dishSpecificationResponse,
                menuScheduleResponse,
                menu.CreatedOn,
                menu.UpdatedOn);
        });

        return menuResponses;
    }
}
