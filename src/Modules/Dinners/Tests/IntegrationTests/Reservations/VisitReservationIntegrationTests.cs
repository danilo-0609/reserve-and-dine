using Dinners.Application.Reservations.Visit;
using Dinners.Domain.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Details;
using Dinners.Domain.Menus.Dishes;
using Dinners.Domain.Menus.Schedules;
using Dinners.Domain.Reservations;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.RestaurantInformations;
using Dinners.Tests.UnitTests.Restaurants;
using Domain.Restaurants;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Dinners.Tests.IntegrationTests.Reservations;

public sealed class VisitReservationIntegrationTests : BaseIntegrationTest
{
    public VisitReservationIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async void Visit_Should_ReturnAnError_WhenReservationDoesNotExist()
    {
        var command = new VisitReservationCommand(Guid.NewGuid());

        var result = await Sender.Send(command);

        bool isErrorReservationNotFound = result
            .FirstError
            .Code == "Reservation.NotFound";
    
        Assert.True(isErrorReservationNotFound);
    }

    [Fact]
    public async void Visit_Should_ReturnAnError_WhenAssistingIsOutOfTimeOfReservationRequested()
    {
        var reservationInformation = ReservationInformation.Create(
            reservedTable: 1,
            25.99m,
            "USD",
            DateTime.Now.AddHours(2).TimeOfDay,
            DateTime.Now.AddHours(2).AddMinutes(45).TimeOfDay,
            DateTime.Now.AddHours(2));

        var reservationAttendees = ReservationAttendees.Create(Guid.NewGuid(),
        "Client name",
        numberOfAttendees: 4);

        var reservation = Reservation.Request(reservationInformation,
            4,
            RestaurantId.CreateUnique(),
            reservationAttendees,
            new List<MenuId>());

        reservation.Value.Pay();

        await DbContext.Reservations.AddAsync(reservation.Value);
        await DbContext.SaveChangesAsync();

        //Trying to visit now. Reservation is in two hours
        var command = new VisitReservationCommand(reservation.Value.Id.Value); 

        var result = await Sender.Send(command);

        bool isErrorMustAssistToReservationInTheRequestedTime = result
            .FirstError
            .Code == "Reservation.MustAssistToReservationInTheRequestedTime";

        Assert.True(isErrorMustAssistToReservationInTheRequestedTime);
    }

    [Fact]
    public async void Visit_Should_ReturnAnError_WhenReservationStatusIsNotPaid()
    {
        var reservationInformation = ReservationInformation.Create(
            reservedTable: 1,
            25.99m,
            "USD",
            DateTime.Now.TimeOfDay,
            DateTime.Now.AddMinutes(45).TimeOfDay,
            DateTime.Now);

        var reservationAttendees = ReservationAttendees.Create(Guid.NewGuid(),
        "Client name",
        numberOfAttendees: 4);

        var reservation = Reservation.Request(reservationInformation,
            4,
            RestaurantId.CreateUnique(),
            reservationAttendees,
            new List<MenuId>());

        await DbContext.Reservations.AddAsync(reservation.Value);
        await DbContext.SaveChangesAsync();

        //ReservationStatus is not Paid
        var command = new VisitReservationCommand(reservation.Value.Id.Value);

        var result = await Sender.Send(command);

        bool isErrorAssistWhenReservationStatusIsNotPaid = result
            .FirstError
            .Code == "Reservation.AssistWhenReservationStatusIsNotPaid";

        Assert.True(isErrorAssistWhenReservationStatusIsNotPaid);
    }

    [Fact]
    public async void Visit_Should_ReturnAnError_WhenRestaurantDoesNotExist()
    {
        var reservationInformation = ReservationInformation.Create(
            reservedTable: 1,
            25.99m,
            "USD",
            DateTime.Now.TimeOfDay,
            DateTime.Now.AddMinutes(45).TimeOfDay,
            DateTime.Now);

        var reservationAttendees = ReservationAttendees.Create(Guid.NewGuid(),
        "Client name",
        numberOfAttendees: 4);

        var reservation = Reservation.Request(reservationInformation,
            4,
            RestaurantId.CreateUnique(),
            reservationAttendees,
            new List<MenuId>());

        reservation.Value.Pay();

        await DbContext.Reservations.AddAsync(reservation.Value);
        await DbContext.SaveChangesAsync();

        var command = new VisitReservationCommand(reservation.Value.Id.Value);

        var result = await Sender.Send(command);

        bool isErrorRestaurantNotFound = result
            .FirstError
            .Code == "Restaurant.NotFound";

        Assert.True(isErrorRestaurantNotFound);
    }

    [Fact]
    public async void Visit_Should_ReturnAnError_WhenRestaurantTableIsOccuppied()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var reservationInformation = ReservationInformation.Create(
            reservedTable: 1,
            25.99m,
            "USD",
            DateTime.Now.TimeOfDay,
            DateTime.Now.AddMinutes(45).TimeOfDay,
            DateTime.Now);

        var reservationAttendees = ReservationAttendees.Create(Guid.NewGuid(),
        "Client name",
        numberOfAttendees: 4);

        var reservation = Reservation.Request(reservationInformation,
            4,
            restaurant.Id,
            reservationAttendees,
            new List<MenuId>());

        reservation.Value.Pay();
        restaurant.OccupyTable(reservation.Value.ReservationInformation.ReservedTable); //Table will be occuppied

        await DbContext.Reservations.AddAsync(reservation.Value);
        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new VisitReservationCommand(reservation.Value.Id.Value);

        var result = await Sender.Send(command);

        bool isErrorRestaurantTableIsNotFree = result
            .FirstError
            .Code == "Restaurant.TableIsNotFree";

        Assert.True(isErrorRestaurantTableIsNotFree);
    }

    [Fact]
    public async void Visit_Should_TurnReservationStatusToVisiting_WhenSuccessful()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var reservationInformation = ReservationInformation.Create(
            reservedTable: 1,
            25.99m,
            "USD",
            DateTime.Now.TimeOfDay,
            DateTime.Now.AddMinutes(45).TimeOfDay,
            DateTime.Now);

        var reservationAttendees = ReservationAttendees.Create(Guid.NewGuid(),
        "Client name",
        numberOfAttendees: 4);

        var reservation = Reservation.Request(reservationInformation,
            4,
            restaurant.Id,
            reservationAttendees,
            new List<MenuId>());

        reservation.Value.Pay();

        await DbContext.Reservations.AddAsync(reservation.Value);
        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new VisitReservationCommand(reservation.Value.Id.Value);

        var result = await Sender.Send(command);

        var reservationModified = await DbContext
            .Reservations
            .Where(r => r.Id == reservation.Value.Id)
            .SingleAsync();

        bool isReservationStatusVisiting = reservationModified.ReservationStatus == ReservationStatus.Visiting;

        Assert.True(isReservationStatusVisiting);
    }

    [Fact]
    public async void Visit_Should_PublishReservationVisitedDomainEvent_ToAddRestaurantClientInDomainEventHandler_WhenSuccessful()
    {
        var restaurantId = RestaurantId.CreateUnique();

        var restaurant = new RestaurantTests().CreateRestaurant(restaurantId);

        var reservationInformation = ReservationInformation.Create(
            reservedTable: 1,
            25.99m,
            "USD",
            DateTime.Now.TimeOfDay,
            DateTime.Now.AddMinutes(45).TimeOfDay,
            DateTime.Now);

        var reservationAttendees = ReservationAttendees.Create(Guid.NewGuid(),
            "Client name",
            numberOfAttendees: 4);

        var reservation = Reservation.Request(reservationInformation,
            4,
            restaurantId,
            reservationAttendees,
            new List<MenuId>());

        reservation.Value.Pay();

        reservation.Value.ClearDomainEvents();
        restaurant.ClearDomainEvents();

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.Reservations.AddAsync(reservation.Value);
        await DbContext.SaveChangesAsync();

        var command = new VisitReservationCommand(reservation.Value.Id.Value);

        await Sender.Send(command);

        await Task.Delay(15_000);

        var restaurantModified = await DbContext
            .Restaurants
            .Where(r => r.Id == restaurant.Id)
            .SingleAsync();

        bool areRestaurantsClientsAdded = restaurantModified.RestaurantClients.Any(); 

        Assert.True(areRestaurantsClientsAdded);
    }

    [Fact]
    public async void Visit_Should_ConsumeMenusInReservation_WhenSuccessful()
    {
        var menuId = MenuId.CreateUnique();

        var restaurantId = RestaurantId.CreateUnique();

        List<MenuSchedule> menuSchedules = new()
        {
            MenuSchedule.Create(DayOfWeek.Tuesday, TimeSpan.FromHours(7), TimeSpan.FromHours(19), menuId),
            MenuSchedule.Create(DayOfWeek.Wednesday, TimeSpan.FromHours(8), TimeSpan.FromHours(19), menuId),
        };

        var menuId2 = MenuId.CreateUnique();

        List<MenuSchedule> menuSchedules2 = new()
        {
            MenuSchedule.Create(DayOfWeek.Tuesday, TimeSpan.FromHours(7), TimeSpan.FromHours(19), menuId2),
            MenuSchedule.Create(DayOfWeek.Wednesday, TimeSpan.FromHours(8), TimeSpan.FromHours(19), menuId2),
        };

        var menuDetails = MenuDetails.Create("Menu Details - Menu title",
            "Menu Details -  description",
            MenuType.Breakfast,
            new Price(15.60m, "USD"),
            0m,
            false,
            "Menu Details - Primary chef name",
            false);

        var dishSpecification = DishSpecification.Create(
            "Menu - Main course",
            "Menu - Side dishes",
            "Menu - Appetizers",
            "Menu - Beverages",
            "Menu - Desserts",
            "Menu - Sauces",
            "Menu - Condiments",
            "Menu - Coffee");

        var menu = Menu.Publish(menuId,
            restaurantId,
            menuDetails,
            dishSpecification,
            new List<string>(),
            new List<string>(),
            new List<string>(),
            menuSchedules,
            DateTime.Now);

        var menu2 = Menu.Publish(menuId2,
            restaurantId,
            menuDetails,
            dishSpecification,
            new List<string>(),
            new List<string>(),
            new List<string>(),
            menuSchedules2,
            DateTime.Now);

        var restaurant = new RestaurantTests().CreateRestaurant(restaurantId);

        var reservationInformation = ReservationInformation.Create(
            reservedTable: 1,
            25.99m,
            "USD",
            DateTime.Now.TimeOfDay,
            DateTime.Now.AddMinutes(45).TimeOfDay,
            DateTime.Now);

        var reservationAttendees = ReservationAttendees.Create(Guid.NewGuid(),
            "Client name",
            numberOfAttendees: 4);

        var reservation = Reservation.Request(reservationInformation,
            4,
            restaurantId,
            reservationAttendees,
            new List<MenuId>() { menuId, menuId2 } );

        reservation.Value.Pay();

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.Reservations.AddAsync(reservation.Value);
        await DbContext.Menus.AddAsync(menu);
        await DbContext.Menus.AddAsync(menu2);
        await DbContext.SaveChangesAsync();

        menu.Consume(Guid.NewGuid());

        DbContext.Menus.Update(menu);
        await DbContext.SaveChangesAsync();

        var command = new VisitReservationCommand(reservation.Value.Id.Value);

        await Sender.Send(command);

        var menuModified = await DbContext.Menus.Where(r => r.Id == menuId).SingleOrDefaultAsync();
        
        var menuModified2 = await DbContext.Menus.Where(r => r.Id == menuId2).SingleOrDefaultAsync();

        bool areMenusConsumed = menuModified!.MenuConsumers.Any() && menuModified2!.MenuConsumers.Any();

        Assert.True(areMenusConsumed);
    }
}
