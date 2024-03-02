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
    private readonly List<MenuConsumer> _menuConsumers = new();

    public new MenuId Id { get; private set; }

    public RestaurantId RestaurantId { get; private set; }

    public MenuDetails MenuDetails { get; private set; }

    public DishSpecification DishSpecification { get; private set; }

    public IReadOnlyList<MenuReviewId> MenuReviewIds => _menuReviewIds.AsReadOnly();

    public MenuSchedule MenuSchedule { get; private set; }

    public IReadOnlyList<MenuConsumer> MenuConsumers => _menuConsumers.AsReadOnly();

    public DateTime CreatedOn { get; private set; }

    public DateTime? UpdatedOn { get; private set; }



    public static Menu Publish(RestaurantId restaurantId,
        MenuDetails menuDetails,
        DishSpecification dishSpecification,
        MenuSchedule menuSchedule,
        DateTime createdOn)
    {
        Menu menu = new Menu(new List<MenuReviewId>(),
            MenuId.CreateUnique(),
            restaurantId,
            menuDetails,
            dishSpecification,
            menuSchedule,
            new List<MenuConsumer>(),
            createdOn,
            null);

        menu.AddDomainEvent(new MenuPublishedDomainEvent(Guid.NewGuid(),
            menu.Id,
            DateTime.UtcNow));

        return menu;
    }

    public MenuDetails UpdateMenuDetails(string title,
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
        var menuSpecification = MenuDetails.Create(title,
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

    public ErrorOr<MenuReview> Review(Guid clientId,
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

        return menuReview;
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

    public MenuSchedule SetMenuSchedule(List<DayOfWeek> days, 
        TimeSpan start, 
        TimeSpan end)
    {
        return MenuSchedule.Create(days, start, end);
    }

    public Menu Update(List<MenuReviewId> menuReviewIds,
        MenuDetails menuSpecification,
        DishSpecification dishSpecification,
        MenuSchedule menuSchedule,
        List<MenuConsumer> menuConsumers,
        DateTime? updatedOn)
    {
        return new Menu(menuReviewIds,
            Id,
            RestaurantId,
            menuSpecification,
            dishSpecification,
            menuSchedule,
            menuConsumers,
            CreatedOn,
            updatedOn);
    }

    public MenuConsumer Consume(Guid clientId)
    {
        MenuConsumer menuConsumer = new(clientId, Id);

        if (_menuConsumers.Any(o => o.MenuId == menuConsumer.MenuId 
            && o.ClientId == menuConsumer.ClientId))
        {
            return _menuConsumers
                .Where(r => r.MenuId == Id && r.ClientId == clientId)
                .Single();
        }

        _menuConsumers.Add(menuConsumer);

        return menuConsumer;
    }
        
    private Menu(List<MenuReviewId> menuReviewIds, 
        MenuId id, 
        RestaurantId restaurantId,
        MenuDetails menuSpecification, 
        DishSpecification dishSpecification, 
        MenuSchedule menuSchedule,
        List<MenuConsumer> menuConsumers,
        DateTime createdOn, 
        DateTime? updatedOn)
    {
        _menuReviewIds = menuReviewIds;
        _menuConsumers = menuConsumers;

        Id = id;
        RestaurantId = restaurantId;
        MenuDetails = menuSpecification;
        DishSpecification = dishSpecification;
        MenuSchedule = menuSchedule;
        CreatedOn = createdOn;
        UpdatedOn = updatedOn;
    }

    private Menu() { }
}
