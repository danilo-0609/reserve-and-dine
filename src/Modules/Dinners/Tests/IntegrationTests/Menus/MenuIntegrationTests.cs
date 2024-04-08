using BuildingBlocks.Application;
using Dinners.Application.Menus.DishSpecifications;
using Dinners.Application.Menus.MenuSchedules;
using Dinners.Application.Menus.MenuSpecification;
using Dinners.Application.Menus.Publish;
using Dinners.Application.Menus.Review;
using Dinners.Domain.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Details;
using Dinners.Domain.Menus.Dishes;
using Dinners.Domain.Menus.MenuReviews;
using Dinners.Domain.Menus.Schedules;
using Dinners.Domain.Restaurants;
using Dinners.Infrastructure.Domain.Menus;
using Dinners.Infrastructure.Domain.Menus.MenuReviews;
using Dinners.Infrastructure.Domain.Menus.Reviews;
using Dinners.Tests.UnitTests.Restaurants;
using Domain.Restaurants;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Dinners.Tests.IntegrationTests.Menus;

public sealed class MenuIntegrationTests : BaseIntegrationTest
{
    private readonly IMenuRepository _menuRepository;
    private readonly IExecutionContextAccessor _executionContextAccessorMock;
    private readonly IReviewRepository _reviewRepository;
    private readonly IMenusReviewsRepository _menusReviewsRepository;

    public MenuIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        _reviewRepository = new ReviewRepository(DbContext);
        _menuRepository = new MenuRepository(DbContext);
        _menusReviewsRepository = new MenusReviewsRepository(DbContext);

        _executionContextAccessorMock = Substitute.For<IExecutionContextAccessor>();
    }

    private readonly MenuDetails _menuDetails = MenuDetails.Create("Menu Details - Menu title",
    "Menu Details -  description",
    MenuType.Breakfast,
    new Price(15.60m, "USD"),
    0m,
    false,
    "Menu Details - Primary chef name",
    false);

    private readonly DishSpecification _dishSpecification = DishSpecification.Create(
        "Menu - Main course",
        "Menu - Side dishes",
        "Menu - Appetizers",
        "Menu - Beverages",
        "Menu - Desserts",
        "Menu - Sauces",
        "Menu - Condiments",
        "Menu - Coffee");

    [Fact]
    public async void Publish_Should_ReturnAnError_WhenRestaurantDoesNotExistInDatabase()
    {
        //Arrange
        var command = new PublishMenuCommand(RestaurantId: Guid.NewGuid(),
            "Menu title",
            "Menu description",
            "Breakfast",
            10.0m,
            "USD",
            0.0m,
            new List<string>() { "#Lunch", "#Delicious" },
            false,
            "Primary chef name",
            false,
            new List<string>() { "Potato", "Tomato" });

        //Act
        var result = await Sender.Send(command);

        bool isErrorRestaurantNotFound = result.FirstError.Code == "Restaurant.NotFound";
        //Assert

        Assert.True(isErrorRestaurantNotFound);
    }

    [Fact]
    public async void Publish_Should_ReturnAValidationError_WhenMenuTypeRequestedIsNotValid()
    {
        var menuType = "Junk food";

        var command = new PublishMenuCommand(RestaurantId: Guid.NewGuid(),
            "Menu title",
            "Menu description",
            menuType,
            10.0m,
            "USD",
            0.0m,
            new List<string>() { "#Lunch", "#Delicious" },
            false,
            "Primary chef name",
            false,
            new List<string>() { "Potato", "Tomato" });

        var result = await Sender.Send(command);

        bool isValidationError = result.FirstError.Type == ErrorOr.ErrorType.Validation
            && result.FirstError.Description == "Menu type must be a valid value";

        Assert.True(isValidationError);
    }

    [Theory]
    [InlineData("Breakfast")]
    [InlineData("Lunch")]
    [InlineData("Dinner")]
    public async void Publish_Should_ReturnAMenuId_WhenSuccessful(string menuType)
    {
        Restaurant restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();
        

        var command = new PublishMenuCommand(restaurant.Id.Value,
            "Menu title",
            "Menu description",
            menuType,
            10.0m,
            "USD",
            0.0m,
            new List<string>() { "#Lunch", "#Delicious" },
            false,
            "Primary chef name",
            false,
            new List<string>() { "Tomato", "Rice", "Potato" });

        var result = await Sender.Send(command);

        Assert.IsType<Guid>(result.Value);
    }

    [Fact]
    public async void Publish_Should_StoreTheMenuInDatabase_WhenSuccessful()
    {
        Restaurant restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();


        var command = new PublishMenuCommand(restaurant.Id.Value,
            "Menu title",
            "Menu description",
            "Lunch",
            10.0m,
            "USD",
            0.0m,
            new List<string>() { "#Lunch", "#Delicious" },
            false,
            "Primary chef name",
            false,
            new List<string>() { "Tomato", "Rice", "Potato" });

        var result = await Sender.Send(command);

        bool menuWasStoredInDatabase = DbContext.Menus.Any(r => r.Id == MenuId.Create(result.Value));

        Assert.True(menuWasStoredInDatabase);
    }

    [Fact]
    public async void UpdateMenuDetails_Should_ReturnAnError_WhenMenuDoesNotExistInDatabase()
    {
        var command = new UpdateMenuDetailsCommand(MenuId: Guid.NewGuid(),
            "Menu Title",
            "Menu description",
            "Lunch",
            "Discount terms",
            29.99m,
            "USD",
            0.0m,
            false,
            "Primary chef name",
            false);

        var result = await Sender.Send(command);

        bool isErrorMenuDoesNotExist = result.FirstError.Code == "Menu.NotFound";

        Assert.True(isErrorMenuDoesNotExist);
    }

    [Fact]
    public async void UpdateMenuDetails_Should_ReturnAValidationError_WhenMenuTypeRequestedIsNotValid()
    {
        var menuType = "Junk food";

        var command = new UpdateMenuDetailsCommand(MenuId: Guid.NewGuid(),
            "Menu Title",
            "Menu description",
            menuType,
            "Discount terms",
            29.99m,
            "USD",
            0.0m,
            false,
            "Primary chef name",
            false);

        var result = await Sender.Send(command);

        bool isValidationError = result.FirstError.Type == ErrorOr.ErrorType.Validation
            && result.FirstError.Description == "Menu type must be a valid value";

        Assert.True(isValidationError);
    }

    [Fact]
    public async void UpdateMenuDetails_Should_UpdateMenuInDatabase_WhenSuccessful()
    {
        var menuId = MenuId.CreateUnique();

        List<MenuSchedule> menuSchedules = new()
        {
            MenuSchedule.Create(DayOfWeek.Tuesday, TimeSpan.FromHours(7), TimeSpan.FromHours(19), menuId),
            MenuSchedule.Create(DayOfWeek.Wednesday, TimeSpan.FromHours(8), TimeSpan.FromHours(19), menuId),
        };

        var menu = Menu.Publish(menuId,
            RestaurantId.CreateUnique(),
            _menuDetails,
            _dishSpecification,
            new List<string>(),
            new List<string>(),
            new List<string>(),
            menuSchedules,
            DateTime.Now);

        await DbContext.Menus.AddAsync(menu);
        await DbContext.SaveChangesAsync();

        DbContext.Entry(menu).State = EntityState.Detached;

        var menuDetails = MenuDetails.Create("Menu Title",
            "Menu description",
            MenuType.Dinner,
            new Price(29.99m, "USD"),
            0.0m,
            false,
            "Primary chef name",
            false);

        var command = new UpdateMenuDetailsCommand(menuId.Value,
            menuDetails.Title,
            menuDetails.Description,
            menuDetails.MenuType.Value,
            menuDetails.DiscountTerms,
            menuDetails.Price.Amount,
            menuDetails.Price.Currency,
            menuDetails.Discount,
            menuDetails.IsVegetarian,
            menuDetails.PrimaryChefName,
            menuDetails.HasAlcohol);

        await Sender.Send(command);

        var menuFromDatabase = await DbContext
            .Menus
            .Where(r => r.Id == menuId)
            .SingleOrDefaultAsync();

        bool isMenuDetailsUpdated = menuFromDatabase!.MenuDetails == menuDetails;

        Assert.True(isMenuDetailsUpdated);
    }

    [Fact]
    public async void Review_Should_ReturnAnError_WhenMenuDoesNotExist()
    {
        var command = new ReviewMenuCommand(Guid.NewGuid(),
            4.5m,
            "Comment");

        var result = await Sender.Send(command);

        bool isErrorMenuDoesNotExist = result.FirstError.Code == "Menu.NotFound";

        Assert.True(isErrorMenuDoesNotExist);   
    }

    [Fact]
    public async void Review_Should_ReturnAReviewId_WhenSuccessful()
    {
        var menuId = MenuId.CreateUnique();

        List<MenuSchedule> menuSchedules = new()
        {
            MenuSchedule.Create(DayOfWeek.Tuesday, TimeSpan.FromHours(7), TimeSpan.FromHours(19), menuId),
            MenuSchedule.Create(DayOfWeek.Wednesday, TimeSpan.FromHours(8), TimeSpan.FromHours(19), menuId),
        };

        var menu = Menu.Publish(menuId,
            RestaurantId.CreateUnique(),
            _menuDetails,
            _dishSpecification,
            new List<string>(),
            new List<string>(),
            new List<string>(),
            menuSchedules,
            DateTime.Now);

        var clientId = Guid.NewGuid();

        menu.Consume(clientId);

        await _menuRepository.AddAsync(menu, CancellationToken.None);
        await DbContext.SaveChangesAsync();
        
        _executionContextAccessorMock.UserId.Returns(clientId);

        var command = new ReviewMenuCommand(menuId.Value,
            4.5m,
            "Comment");

        var handler = new ReviewMenuCommandHandler(_reviewRepository, 
            _menuRepository, 
            _executionContextAccessorMock,
            _menusReviewsRepository);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.IsType<Guid>(result.Value);
    }

    [Fact]
    public async void UpdateDishSpecification_Should_ReturnAnError_WhenMenuDoesNotExist()
    {
        var command = new UpdateDishSpecificationCommand(Guid.NewGuid(),
            "Main course",
            "Side dishes",
            "Appetizers",
            "Menu beverages",
            "Menu desserts",
            "Sauces",
            "Condiments",
            "Menu Coffee");

        var result = await Sender.Send(command);

        bool isErrorMenuDoesNotExist = result.FirstError.Code == "Menu.NotFound";

        Assert.True(isErrorMenuDoesNotExist);
    }

    [Fact]
    public async void UpdateDishSpecification_Should_UpdateMenuInDatabase_WhenSuccessful()
    {
        var menuId = MenuId.CreateUnique();

        List<MenuSchedule> menuSchedules = new()
        {
            MenuSchedule.Create(DayOfWeek.Tuesday, TimeSpan.FromHours(7), TimeSpan.FromHours(19), menuId),
            MenuSchedule.Create(DayOfWeek.Wednesday, TimeSpan.FromHours(8), TimeSpan.FromHours(19), menuId),
        };

        var menu = Menu.Publish(menuId,
            RestaurantId.CreateUnique(),
            _menuDetails,
            _dishSpecification,
            new List<string>(),
            new List<string>(),
            new List<string>(),
            menuSchedules,
            DateTime.Now);

        await DbContext.Menus.AddAsync(menu);
        await DbContext.SaveChangesAsync();

        DbContext.Entry(menu).State = EntityState.Detached;

        var dishSpecification = DishSpecification.Create("Main course",
            "Side dishes",
            "Appetizers",
            "Menu beverages",
            "Menu desserts",
            "Sauces",
            "Condiments",
            "Menu Coffee");

        var command = new UpdateDishSpecificationCommand(menuId.Value,
            dishSpecification.MainCourse,
            dishSpecification.SideDishes,
            dishSpecification.Appetizers,
            dishSpecification.Beverages,
            dishSpecification.Desserts,
            dishSpecification.Sauces,
            dishSpecification.Condiments,
            dishSpecification.Coffee);

        await Sender.Send(command);

        var menuFromDatabase = await DbContext
            .Menus
            .Where(r => r.Id == menuId)
            .SingleOrDefaultAsync();

        bool isDishSpecificationUpdated = menuFromDatabase!.DishSpecification == dishSpecification;
            
        Assert.True(isDishSpecificationUpdated);
    }

    [Fact]
    public async void SetMenuSchedule_Should_ReturnAnError_WhenMenuDoesNotExistInDatabase()
    {
        var command = new SetMenuScheduleCommand(Guid.NewGuid(),
            DayOfWeek.Tuesday,
            TimeSpan.FromHours(8),
            TimeSpan.FromHours(19));

        var result = await Sender.Send(command);

        bool isErrorMenuDoesNotExist = result.FirstError.Code == "Menu.NotFound";

        Assert.True(isErrorMenuDoesNotExist);
    }

    [Fact]
    public async void SetMenuSchedule_Should_AddMenuScheduleInDatabase_WhenSuccessful()
    {
        var menuId = MenuId.CreateUnique();

        List<MenuSchedule> menuSchedules = new()
        {
            MenuSchedule.Create(DayOfWeek.Tuesday, TimeSpan.FromHours(7), TimeSpan.FromHours(19), menuId),
            MenuSchedule.Create(DayOfWeek.Wednesday, TimeSpan.FromHours(8), TimeSpan.FromHours(19), menuId),
        };

        var menu = Menu.Publish(menuId,
            RestaurantId.CreateUnique(),
            _menuDetails,
            _dishSpecification,
            new List<string>(),
            new List<string>(),
            new List<string>(),
            menuSchedules,
            DateTime.Now);

        await DbContext.Menus.AddAsync(menu);
        await DbContext.SaveChangesAsync();

        var command = new SetMenuScheduleCommand(menu.Id.Value,
            DayOfWeek.Saturday,
            TimeSpan.FromHours(8),
            TimeSpan.FromHours(19));

        var result = await Sender.Send(command);

        bool isMenuScheduleStoredInDatabase = DbContext
            .Menus
            .Any(r => r.MenuSchedules
                .Any(t => t.Day == DayOfWeek.Saturday));

        Assert.True(isMenuScheduleStoredInDatabase);
    }
}
