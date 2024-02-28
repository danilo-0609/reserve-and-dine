using BuildingBlocks.Domain.AggregateRoots;
using Dinners.Domain.Common;
using Dinners.Domain.Menus.Events;
using Dinners.Domain.Menus.MenuReviews;
using Dinners.Domain.Restaurants;
using ErrorOr;

namespace Dinners.Domain.Menus;

public sealed class Menu : AggregateRoot<MenuId, Guid>
{
    private readonly List<MenuReviewId> _menuReviewIds = new();
    private readonly List<MenuConsumer> _menuConsumer = new();

    public new MenuId Id { get; private set; }

    public RestaurantId RestaurantId { get; private set; }

    public MenuSpecification MenuSpecification { get; private set; }

    public DishSpecification DishSpecification { get; private set; }

    public IReadOnlyList<MenuReviewId> MenuReviewIds => _menuReviewIds.AsReadOnly();

    public MenuSchedule MenuSchedule { get; private set; }

    public IReadOnlyList<MenuConsumer> MenuConsumers => _menuConsumer.AsReadOnly();

    public DateTime CreatedOn { get; private set; }

    public DateTime? UpdatedOn { get; private set; }



    public static Menu Publish(RestaurantId restaurantId,
        MenuSpecification menuSpecification,
        DishSpecification dishSpecification,
        MenuSchedule menuSchedule,
        DateTime createdOn)
    {
        Menu menu = new Menu(new List<MenuReviewId>(),
            MenuId.CreateUnique(),
            restaurantId,
            menuSpecification,
            dishSpecification,
            menuSchedule,
            createdOn,
            null);

        menu.AddDomainEvent(new MenuPublishedDomainEvent(Guid.NewGuid(),
            menu.Id,
            DateTime.UtcNow));

        return menu;
    }

    public MenuSpecification UpdateMenuSpecification(string title,
        string description,
        MenuType menuType,
        Price price,
        decimal discount,
        List<string?> menuImagesUrl,
        List<string?> tags,
        bool isVegetarian,
        string primaryChefName,
        bool hasAlcohol,
        string discountTerms = "")
    {
        var menuSpecification = MenuSpecification.Create(title,
            description,
            menuType,
            price,
            discount,
            menuImagesUrl,
            tags,
            isVegetarian,
            primaryChefName,
            hasAlcohol,
            discountTerms);

        return menuSpecification;
    }

    public ErrorOr<MenuReviewId> Review(Guid clientId,
        decimal rate,
        List<MenuConsumer> menuConsumers,
        DateTime reviewedAt,
        string comment = "")
    {
        var menuReview = MenuReview.Post(clientId, rate, menuConsumers, reviewedAt, comment);

        if (menuReview.IsError)
        {
            return menuReview.FirstError;
        }

        _menuReviewIds.Add(menuReview.Value.Id);

        return menuReview.Value.Id;
    }

    public DishSpecification UpdateDishSpecification(List<string> ingredients,
        string mainCourse = "",
        string sideDishes = "",
        string appetizers = "",
        string beverages = "",
        string desserts = "",
        string sauces = "",
        string condiments = "",
        string coffee = "")
    {
        return DishSpecification.Create(
            ingredients,
            mainCourse,
            sideDishes,
            appetizers,
            beverages,
            desserts,
            sauces,
            condiments,
            coffee);
    }

    public MenuSchedule SetMenuSchedule(List<DayOfWeek> days, TimeSpan start, TimeSpan end)
    {
        return MenuSchedule.Create(days, start, end);
    }

    public Menu Update(List<MenuReviewId> menuReviewIds,
        MenuSpecification menuSpecification,
        DishSpecification dishSpecification,
        MenuSchedule menuSchedule,
        DateTime? updatedOn)
    {
        return new Menu(menuReviewIds,
            Id,
            RestaurantId,
            menuSpecification,
            dishSpecification,
            menuSchedule,
            CreatedOn,
            updatedOn);
    }

    public MenuConsumer Consume(Guid clientId)
    {
        return new MenuConsumer(clientId, Id);
    }

    private Menu(List<MenuReviewId> menuReviewIds, 
        MenuId id, 
        RestaurantId restaurantId,
        MenuSpecification menuSpecification, 
        DishSpecification dishSpecification, 
        MenuSchedule menuSchedule, 
        DateTime createdOn, 
        DateTime? updatedOn)
    {
        _menuReviewIds = menuReviewIds;
        Id = id;
        RestaurantId = restaurantId;
        MenuSpecification = menuSpecification;
        DishSpecification = dishSpecification;
        MenuSchedule = menuSchedule;
        CreatedOn = createdOn;
        UpdatedOn = updatedOn;
    }

    private Menu() { }
}
