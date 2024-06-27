using Dinners.Application.Reservations.Finish;
using Dinners.Domain.Menus;
using Dinners.Domain.Reservations;
using Dinners.Domain.Restaurants;
using Dinners.Tests.UnitTests.Restaurants;
using Microsoft.EntityFrameworkCore;

namespace Dinners.Tests.IntegrationTests.Reservations;

public sealed class FinishReservationIntegrationTests : BaseIntegrationTest
{
    public FinishReservationIntegrationTests(IntegrationTestWebAppFactory factory) 
        : base(factory)
    {
    }

    [Fact]
    public async void Finish_Should_ReturnAnError_WhenReservationDoesNotExist()
    {
        var command = new FinishReservationCommand(Guid.NewGuid());

        var result = await Sender.Send(command);

        bool isErrorReservationNotFound = result.FirstError.Code == "Reservation.NotFound";

        Assert.True(isErrorReservationNotFound);
    }

    [Fact]
    public async void Finish_Should_ReturnAnError_WhenRestaurantDoesNotExist()
    {
        var reservationInformation = ReservationInformation.Create(
            reservedTable: 1,
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

        reservation.Value.Visit(reservation.Value.ReservationAttendees.ClientId);

        await DbContext.Reservations.AddAsync(reservation.Value);
        await DbContext.SaveChangesAsync();

        var command = new FinishReservationCommand(reservation.Value.Id.Value);

        var result = await Sender.Send(command);

        bool isErrorRestaurantWasNotFound = result
            .FirstError
            .Code == "Restaurant.NotFound";

        Assert.True(isErrorRestaurantWasNotFound);
    }

    [Fact]
    public async void Finish_Should_ReturnAnError_WhenReservationStatusIsNotAsisting()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var reservationInformation = ReservationInformation.Create(
            reservedTable: 1,
            DateTime.Now.AddHours(2).TimeOfDay,
            DateTime.Now.AddHours(2).AddMinutes(45).TimeOfDay,
            DateTime.Now.AddHours(2));

        var reservationAttendees = ReservationAttendees.Create(Guid.NewGuid(),
            "Client name",
            numberOfAttendees: 4);

        var reservation = Reservation.Request(reservationInformation,
            4,
            restaurant.Id,
            reservationAttendees,
            new List<MenuId>());

        await DbContext.Reservations.AddAsync(reservation.Value);
        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new FinishReservationCommand(reservation.Value.Id.Value);

        var result = await Sender.Send(command);

        bool isErrorCannotFinishIfStatusIsNotAssisting = result
            .FirstError
            .Code == "Reservation.CannotFinishIfStatusIsNotAssisting";

        Assert.True(isErrorCannotFinishIfStatusIsNotAssisting);
    }

    [Fact]
    public async void Finish_Should_ReturnAnError_WhenRestaurantTableIsNotOccupiedNow()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var reservationInformation = ReservationInformation.Create(
            reservedTable: 1,
            DateTime.Now.AddHours(2).TimeOfDay,
            DateTime.Now.AddHours(2).AddMinutes(45).TimeOfDay,
            DateTime.Now.AddHours(2));

        var reservationAttendees = ReservationAttendees.Create(Guid.NewGuid(),
            "Client name",
            numberOfAttendees: 4);

        var reservation = Reservation.Request(reservationInformation,
            4,
            restaurant.Id,
            reservationAttendees,
            new List<MenuId>());

        reservation.Value.Visit(reservation.Value.ReservationAttendees.ClientId);

        await DbContext.Reservations.AddAsync(reservation.Value);
        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new FinishReservationCommand(reservation.Value.Id.Value);

        var result = await Sender.Send(command);

        bool isErrorCannotFreeTableWhenTableIsNotOccupied = result
            .FirstError
            .Code == "Restaurant.CannotFreeTableWhenTableIsNotOccupied";

        Assert.True(isErrorCannotFreeTableWhenTableIsNotOccupied);
    }

    [Fact]
    public async void Finish_Should_TurnReservationStatusToFinished_WhenSuccessful()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var reservationInformation = ReservationInformation.Create(
            reservedTable: 1,
            DateTime.Now.AddMinutes(1).TimeOfDay,
            DateTime.Now.AddMinutes(1).AddMinutes(45).TimeOfDay,
            DateTime.Now.AddMinutes(1));

        var reservationAttendees = ReservationAttendees.Create(Guid.NewGuid(),
            "Client name",
            numberOfAttendees: 4);

        var reservation = Reservation.Request(reservationInformation,
            4,
            restaurant.Id,
            reservationAttendees,
            new List<MenuId>());

        reservation.Value.Visit(reservation.Value.ReservationAttendees.ClientId);

        restaurant.OccupyTable(reservation.Value.ReservationInformation.ReservedTable);

        await DbContext.Reservations.AddAsync(reservation.Value);
        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new FinishReservationCommand(reservation.Value.Id.Value);

        var result = await Sender.Send(command);

        bool isReservationStatusFinished = await DbContext
            .Reservations
            .AnyAsync(r => r.Id == reservation.Value.Id && r.ReservationStatus == ReservationStatus.Finished);

        Assert.True(isReservationStatusFinished);
    }

    [Fact]
    public async void Finish_Should_TurnIsOcuppiedPropertyInRestaurantTableToFalse_WhenSuccessful()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var reservationInformation = ReservationInformation.Create(
            reservedTable: 1,
            DateTime.Now.AddMinutes(1).TimeOfDay,
            DateTime.Now.AddMinutes(1).AddMinutes(45).TimeOfDay,
            DateTime.Now.AddMinutes(1));

        var reservationAttendees = ReservationAttendees.Create(Guid.NewGuid(),
            "Client name",
            numberOfAttendees: 4);

        var reservation = Reservation.Request(reservationInformation,
            4,
            restaurant.Id,
            reservationAttendees,
            new List<MenuId>());

        reservation.Value.Visit(reservation.Value.ReservationAttendees.ClientId);

        restaurant.OccupyTable(reservation.Value.ReservationInformation.ReservedTable);

        await DbContext.Reservations.AddAsync(reservation.Value);
        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new FinishReservationCommand(reservation.Value.Id.Value);

        var result = await Sender.Send(command);

        var restaurantModified = await DbContext
            .Restaurants
            .Where(r => r.Id == restaurant.Id)
            .SingleAsync();

        bool isTableFree = restaurantModified
            .RestaurantTables
            .Any(r => r.Number == reservation.Value.ReservationInformation.ReservedTable && 
            r.IsOccupied == false);

        Assert.True(isTableFree);
    }
}
