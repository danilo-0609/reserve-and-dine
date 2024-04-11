using Dinners.Application.Common;
using Dinners.Application.Menus.GetReviewsByMenuId;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Errors;
using ErrorOr;

namespace Dinners.Application.Menus.GetByIngredients;

internal sealed class GetMenusByIngredientQueryHandler : IQueryHandler<GetMenusByIngredientQuery, ErrorOr<List<MenuResponse>>>
{
    private readonly IMenuRepository _menuRepository;

    public GetMenusByIngredientQueryHandler(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<ErrorOr<List<MenuResponse>>> Handle(GetMenusByIngredientQuery request, CancellationToken cancellationToken)
    {
        List<Menu> menus = await _menuRepository.GetMenusByIngredientAsync(request.Ingredient, cancellationToken);

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
            menu.Tags.ToList().ConvertAll(tag => tag.Value),
            menu.MenuDetails.IsVegetarian,
            menu.MenuDetails.PrimaryChefName,
            menu.MenuDetails.HasAlcohol,
            menu.MenuDetails.DiscountTerms);

            var dishSpecificationResponse = new DishSpecificationResponse(
                menu.Ingredients.ToList().ConvertAll(ingredient => ingredient.Value),
                menu.DishSpecification.MainCourse,
                menu.DishSpecification.SideDishes,
                menu.DishSpecification.Appetizers,
                menu.DishSpecification.Beverages,
                menu.DishSpecification.Desserts,
                menu.DishSpecification.Sauces,
                menu.DishSpecification.Condiments,
                menu.DishSpecification.Coffee);

            List<MenuScheduleResponse> menuSchedulesResponse = menu
                .MenuSchedules
                .ToList()
                .ConvertAll(schedule => new MenuScheduleResponse(schedule.Day,
                    schedule.StartTimeSpan,
                    schedule.EndTimeSpan));

            return new MenuResponse(menu.Id.Value,
                menu.RestaurantId.Value,
                menuDetailsResponse,
                dishSpecificationResponse,
                menuSchedulesResponse,
                menu.MenuImagesUrl.ConvertAll(r => r.Id.Value),
                menu.CreatedOn,
                menu.UpdatedOn);
        });

        return menuResponses;
    }
}
