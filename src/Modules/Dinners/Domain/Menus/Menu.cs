using BuildingBlocks.Domain.AggregateRoots;
using Dinners.Domain.Common;
using Dinners.Domain.Menus.Details;
using Dinners.Domain.Menus.Dishes;
using Dinners.Domain.Menus.Events;
using Dinners.Domain.Menus.MenuReviews;
using Dinners.Domain.Menus.Schedules;
using Dinners.Domain.Restaurants;
using ErrorOr;

namespace Dinners.Domain.Menus;

public sealed class Menu : AggregateRoot<MenuId, Guid>
{
    private readonly List<MenuReviewId> _menuReviewIds = new();
    private readonly List<MenuConsumer> _menuConsumers = new();
    private readonly List<MenuImageUrl> _menuImagesUrl = new();
    private readonly List<Tag> _tags = new();
    private readonly List<Ingredient> _ingredients = new();
    private readonly List<MenuSchedule> _menuSchedules = new();

    public new MenuId Id { get; private set; }

    public RestaurantId RestaurantId { get; private set; }

    public MenuDetails MenuDetails { get; private set; }

    public DishSpecification DishSpecification { get; private set; }

    public List<MenuReviewId> MenuReviewIds => _menuReviewIds;

    public List<MenuImageUrl> MenuImagesUrl => _menuImagesUrl;

    public List<Tag> Tags => _tags;

    public List<Ingredient> Ingredients => _ingredients;

    public List<MenuSchedule> MenuSchedules => _menuSchedules;

    public List<MenuConsumer> MenuConsumers => _menuConsumers;

    public DateTime CreatedOn { get; private set; }

    public DateTime? UpdatedOn { get; private set; }


    public static Menu Publish(MenuId menuId,
        RestaurantId restaurantId,
        MenuDetails menuDetails,
        DishSpecification dishSpecification,
        List<string> imagesUrl,
        List<string> tags,
        List<string> ingredients,
        List<MenuSchedule> menuSchedules,
        DateTime createdOn)
    {
        Menu menu = new Menu(new List<MenuReviewId>(),
            menuId,
            restaurantId,
            menuDetails,
            dishSpecification,
            new List<MenuConsumer>(),
            imagesUrl.ConvertAll(image => new MenuImageUrl(MenuImageUrlId.CreateUnique(), image, menuId)),
            tags.ConvertAll(tag => new Tag(TagId.CreateUnique(), tag, menuId)),
            menuSchedules,
            ingredients.ConvertAll(ingredient => new Ingredient(IngredientId.CreateUnique(), ingredient, menuId)),
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

    public DishSpecification UpdateDishSpecification(string mainCourse = "",
        string sideDishes = "",
        string appetizers = "",
        string beverages = "",
        string desserts = "",
        string sauces = "",
        string condiments = "",
        string coffee = "")
    {
        return DishSpecification.Create(
            mainCourse,
            sideDishes,
            appetizers,
            beverages,
            desserts,
            sauces,
            condiments,
            coffee);
    }

    public MenuSchedule SetMenuSchedule(DayOfWeek day,
        TimeSpan start,
        TimeSpan end)
    {
        var menuSchedule = MenuSchedule.Create(day, start, end, Id);

        _menuSchedules.Add(menuSchedule);

        return menuSchedule;
    }

    public void ModifyMenuSchedule(DayOfWeek day,
        TimeSpan start,
        TimeSpan end)
    {
        var menuSchedule = _menuSchedules
            .Where(r => r.Day == day)
            .Single();

        _menuSchedules.Remove(menuSchedule);

        _menuSchedules.Add(MenuSchedule.Create(day, start, end, Id));
    }

    public void DeleteMenuSchedule(DayOfWeek day)
    {
        var menuSchedule = _menuSchedules
            .Where(r => r.Day == day)
            .Single();

        _menuSchedules.Remove(menuSchedule);
    }

    public Menu Update(List<MenuReviewId> menuReviewIds,
        MenuDetails menuSpecification,
        DishSpecification dishSpecification,
        List<MenuConsumer> menuConsumers,
        List<MenuImageUrl> menuImageUrls,
        List<Tag> tags,
        List<MenuSchedule> menuSchedules,
        List<Ingredient> ingredients,
        DateTime? updatedOn)
    {
        return new Menu(menuReviewIds,
            Id,
            RestaurantId,
            menuSpecification,
            dishSpecification,
            menuConsumers,
            menuImageUrls,
            tags,
            menuSchedules,
            ingredients,
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

    public void AddImage(string imageUrl)
    {
        _menuImagesUrl.Add(new MenuImageUrl(MenuImageUrlId.CreateUnique(), imageUrl, Id));
    }

    public void DeleteImage(string imageUrl, MenuImageUrlId menuImageUrlId)
    {
        _menuImagesUrl.Remove(new MenuImageUrl(menuImageUrlId, imageUrl, Id));
    }

    public void AddTag(string tag)
    {
        _tags.Add(new Tag(TagId.CreateUnique(), tag, Id));
    }

    public void DeleteTag(string tag, TagId tagId)
    {
        _tags.Remove(new Tag(tagId, tag, Id));
    }

    public void AddIngredient(string ingredient)
    {
        _ingredients.Add(new Ingredient(IngredientId.CreateUnique(), ingredient, Id));
    }

    public void DeleteIngredient(string ingredient, IngredientId ingredientId)
    {
        _ingredients.Remove(new Ingredient(ingredientId, ingredient, Id));
    }

    private Menu(List<MenuReviewId> menuReviewIds, 
        MenuId id, 
        RestaurantId restaurantId,
        MenuDetails menuSpecification, 
        DishSpecification dishSpecification,
        List<MenuConsumer> menuConsumers,
        List<MenuImageUrl> imagesUrl,
        List<Tag> tags,
        List<MenuSchedule> menuSchedules,
        List<Ingredient> ingredients,
        DateTime createdOn, 
        DateTime? updatedOn)
    {
        _menuReviewIds = menuReviewIds;
        _menuConsumers = menuConsumers;
        _menuImagesUrl = imagesUrl;
        _tags = tags;
        _menuSchedules = menuSchedules;
        _ingredients = ingredients;

        Id = id;
        RestaurantId = restaurantId;
        MenuDetails = menuSpecification;
        DishSpecification = dishSpecification;
        CreatedOn = createdOn;
        UpdatedOn = updatedOn;
    }

    private Menu() { }
}
