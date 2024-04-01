using Dinners.Application.Menus.MenuSpecification;
using Dinners.Application.Menus.Publish;
using Dinners.Application.Menus.Review;
using Dinners.Domain.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Details;
using Dinners.Domain.Menus.Dishes;
using Dinners.Domain.Menus.Schedules;
using Dinners.Domain.Restaurants;

namespace Dinners.Tests.IntegrationTests.Menus;

public sealed class MenuIntegrationTests : BaseIntegrationTest
{
    public MenuIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    private readonly MenuDetails _menuDetails = MenuDetails.Create("Menu title",
    "Menu description",
    MenuType.Breakfast,
    new Price(15.60m, "USD"),
    0m,
    false,
    "Primary chef name",
    false);

    private readonly DishSpecification _dishSpecification = DishSpecification.Create(
        "Main course",
        "Side dishes",
        "Appetizers",
        "Beverages",
        "Desserts",
        "Sauces",
        "Condiments",
        "Coffee");

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
        var command = new PublishMenuCommand(Guid.NewGuid(),
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

        bool isErrorRestaurantNotFound = result.FirstError.Code == "Restaurant.NotFound";

        Assert.True(isErrorRestaurantNotFound);
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
            new List<string>() { "#Lunch", "#Delicious" },
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
            new List<string>() { "#Lunch", "#Delicious" },
            false,
            "Primary chef name",
            false);

        var result = await Sender.Send(command);

        bool isValidationError = result.FirstError.Type == ErrorOr.ErrorType.Validation
            && result.FirstError.Description == "Menu type must be a valid value";

        Assert.True(isValidationError);
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
}
