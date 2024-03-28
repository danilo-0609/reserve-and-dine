using Dinners.Domain.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Details;
using Dinners.Domain.Menus.Dishes;
using Dinners.Domain.Menus.Events;
using Dinners.Domain.Menus.MenuReviews;
using Dinners.Domain.Menus.Schedules;
using Dinners.Domain.Restaurants;

namespace Dinners.Tests.UnitTests.Menus;

public sealed class MenuTests
{

    private readonly MenuDetails _menuDetails = MenuDetails.Create("Menu title",
        "Menu description",
        MenuType.Breakfast,
        new Price(15.60m, "USD"),
        0m,
        new List<string>(),
        new List<string>(),
        false,
        "Primary chef name",
        false);

    private readonly DishSpecification _dishSpecification = DishSpecification.Create(
        new List<string>(),
        "Main course",
        "Side dishes",
        "Appetizers",
        "Beverages",
        "Desserts",
        "Sauces",
        "Condiments",
        "Coffee");

    private readonly MenuSchedule _menuSchedule = MenuSchedule.Create(new List<DayOfWeek>(), TimeSpan.FromHours(7), TimeSpan.FromHours(19).Add(TimeSpan.FromMinutes(30)));

    [Fact]
    public void Publish_Should_RaiseAMenuPublishedDomainEvent_WhenSuccessful()
    {
        var menu = Menu.Publish(RestaurantId.CreateUnique(),
            _menuDetails,
            _dishSpecification,
            _menuSchedule,
            DateTime.Now);

        bool hasRaisedMenuPublishedDomainEvent = menu
            .DomainEvents
            .Any(g => g.GetType() == typeof(MenuPublishedDomainEvent));

        Assert.True(hasRaisedMenuPublishedDomainEvent);
    }

    [Fact]
    public void Review_Should_ReturnAnError_WhenUserHasNotConsumedTheMenu()
    {
        var menu = Menu.Publish(RestaurantId.CreateUnique(),
            _menuDetails,
            _dishSpecification,
            _menuSchedule,
            DateTime.Now);

        var clientId = Guid.NewGuid();

        var review = menu.Review(clientId, 4.5m, menu.MenuConsumers.ToList(), DateTime.Now, "Review comment");

        bool isErrorCannotReviewWhenUserHasNotConsumedTheMenu = review
            .FirstError
            .Code == "MenuReviews.CannotReviewWhenUserHasNotConsumedTheMenu";

        Assert.True(isErrorCannotReviewWhenUserHasNotConsumedTheMenu);
    }

    [Fact]
    public void Review_Should_ReturnAnError_WhenRateIsNotAValidNumber()
    {
        var menu = Menu.Publish(RestaurantId.CreateUnique(),
            _menuDetails,
            _dishSpecification,
            _menuSchedule,
            DateTime.Now);

        var clientId = Guid.NewGuid();

        menu.Consume(clientId);

        decimal rate = 6m;

        var review = menu.Review(clientId, rate, menu.MenuConsumers.ToList(), DateTime.Now, "Review comment");

        bool isErrorRateIsNotAValidNumber = review
            .FirstError
            .Code == "MenuReviews.RateIsNotAValidNumber";

        Assert.True(isErrorRateIsNotAValidNumber);
    }

    [Fact]
    public void Review_Should_ReturnAMenuReviewEntity_WhenSuccessful()
    {
        var menu = Menu.Publish(RestaurantId.CreateUnique(),
            _menuDetails,
            _dishSpecification,
            _menuSchedule,
            DateTime.Now);

        var clientId = Guid.NewGuid();

        menu.Consume(clientId);

        decimal rate = 4.5m;

        var review = menu.Review(clientId, rate, menu.MenuConsumers.ToList(), DateTime.Now, "Review comment");

        bool isReturningAMenuReviewEntity = review
            .Value
            .GetType() == typeof(MenuReview);

        Assert.True(isReturningAMenuReviewEntity);
    }
}
