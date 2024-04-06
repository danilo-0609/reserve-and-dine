using Dinners.Application.Reservations.Request;
using Dinners.Domain.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Details;
using Dinners.Domain.Menus.Dishes;
using Dinners.Domain.Menus.Schedules;
using Dinners.Tests.UnitTests.Restaurants;

namespace Dinners.Tests.IntegrationTests.Reservations;

public sealed class ReservationIntegrationTests : BaseIntegrationTest
{
    public ReservationIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async void Request_Should_ReturnAnError_WhenRestaurantDoesNotExistInDatabase()
    {
        var command = new RequestReservationCommand(1,
            20.23m,
            "USD",
            DateTime.Now.AddHours(10),
            DateTime.Now.AddHours(10).AddMinutes(45),
            RestaurantId: Guid.NewGuid(),
            "Customer name",
            4,
            new List<Guid>());

        var result = await Sender.Send(command);

        bool isErrorRestaurantDoesNotExist = result.FirstError.Code == "Restaurant.NotFound";

        Assert.True(isErrorRestaurantDoesNotExist);
    }

    [Fact]
    public async void Request_Should_ReturnAnError_WhenAnyMenuIdDoesNotExistInDatabase()
    {
        var restaurant = new RestaurantTests().CreateRestaurant();

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new RequestReservationCommand(1,
            20.23m,
            "USD",
            DateTime.Now.AddHours(10),
            DateTime.Now.AddHours(10).AddMinutes(45),
            RestaurantId: restaurant.Id.Value,
            "Customer name",
            4,
            new List<Guid>() { Guid.NewGuid(), Guid.NewGuid() });

        var result = await Sender.Send(command);

        bool isErrorMenuNotFound = result.FirstError.Code == "Menu.NotFound";

        Assert.True(isErrorMenuNotFound);
    }

    [Fact]
    public async void Request_Should_ReturnAnError_WhenRestaurantTableDoesNotExistInRestaurant()
    {
        //Restaurant has three tables: 1, 2, 3
        var restaurant = new RestaurantTests().CreateRestaurant();

        var menu = Menu.Publish(MenuId.CreateUnique(),
            restaurant.Id,
            MenuDetails.Create("Title", 
                "Description", 
                MenuType.Lunch, 
                new Price(10.23m, "USD"), 
                0.0m, 
                false, 
                "Chef name", 
                false),
            DishSpecification.Create(),
            new List<string>(),
            new List<string>(),
            new List<string>(),
            new List<MenuSchedule>(),
            DateTime.UtcNow);

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.Menus.AddAsync(menu);
        await DbContext.SaveChangesAsync();

        var command = new RequestReservationCommand(ReservedTable: 50,
            20.23m,
            "USD",
            DateTime.Now.AddHours(10),
            DateTime.Now.AddHours(10).AddMinutes(45),
            RestaurantId: restaurant.Id.Value,
            "Customer name",
            4,
            new List<Guid>() { menu.Id.Value });

        var result = await Sender.Send(command);

        bool isErrorTableDoesNotExist = result.FirstError.Code == "Reservation.TableNotFound";

        Assert.True(isErrorTableDoesNotExist);
    }
}
