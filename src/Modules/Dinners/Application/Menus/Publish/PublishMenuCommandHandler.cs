using Dinners.Application.Common;
using Dinners.Domain.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using ErrorOr;

namespace Dinners.Application.Menus.Publish;

internal sealed class PublishMenuCommandHandler : ICommandHandler<PublishMenuCommand, ErrorOr<Guid>>
{
    private readonly IMenuRepository _menuRepository;
    private readonly IRestaurantRepository _restaurantRepository;

    public PublishMenuCommandHandler(IMenuRepository menuRepository, IRestaurantRepository restaurantRepository)
    {
        _menuRepository = menuRepository;
        _restaurantRepository = restaurantRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(PublishMenuCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantRepository.ExistsAsync(RestaurantId.Create(request.RestaurantId));
    
        if (restaurant is false)
        {
            return RestaurantErrorCodes.NotFound;
        }

        var menuType = GetMenuType(request.MenuType);

        MenuDetails menuDetails = MenuDetails.Create(request.Title,
            request.Description,
            menuType,
            new Price(request.Price, request.Currency),
            request.Discount,
            new List<Uri>(),
            request.Tags,
            request.IsVegetarian,
            request.PrimaryChefName,
            request.HasAlcohol,
            request.DiscountTerms);


        DishSpecification dishSpecification = DishSpecification.Create(request.Ingredients,
            request.MainCourse,
            request.SideDishes,
            request.Appetizers,
            request.Beverages,
            request.Desserts,
            request.Sauces,
            request.Condiments,
            request.Coffee);

        MenuSchedule menuSchedule = MenuSchedule.Create(request.DayOfWeeks,
            request.Start,
            request.End);

        Menu menu = Menu.Publish(
            RestaurantId.Create(request.RestaurantId),
            menuDetails,
            dishSpecification,
            menuSchedule,
            DateTime.UtcNow);

        await _menuRepository.AddAsync(menu, cancellationToken);

        return menu.Id.Value;
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
