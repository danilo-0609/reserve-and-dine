﻿using Dinners.Domain.Menus;
using Dinners.Domain.Reservations;
using Dinners.Domain.Reservations.DomainEvents;
using Dinners.Domain.Restaurants;
using Dinners.Tests.UnitTests.Restaurants;

namespace Dinners.Tests.UnitTests.Reservations;

public sealed class ReservationTests
{
    private readonly ReservationInformation _reservationInformation = ReservationInformation.Create(
        reservedTable: 1,
        DateTime.Now.AddHours(2).TimeOfDay,
        DateTime.Now.AddHours(2).AddMinutes(45).TimeOfDay,
        DateTime.Now.AddHours(2));

    private readonly ReservationAttendees _reservationAttendees = ReservationAttendees.Create(Guid.NewGuid(),
        "Client name",
        numberOfAttendees: 4);


    [Fact]
    public void Request_Should_ReturnAnError_WhenNumberOfAttendeesIsGreaterThanNumberOfSeats()
    {
        var reservationAttendees = ReservationAttendees.Create(Guid.NewGuid(),
        "Client name",
        numberOfAttendees: 5);

        var reservation = Reservation.Request(_reservationInformation,
            4,
            RestaurantId.CreateUnique(),
            reservationAttendees,
            new List<MenuId>());

        bool isErrorCannotReserveWhenNumberOfAttendeesIsGreaterThanSeatsOfTableReserved = reservation
            .FirstError
            .Code == "Reservation.CannotReserveWhenNumberOfAttendeesIsGreaterThanSeatsOfTableReserved";

        Assert.True(isErrorCannotReserveWhenNumberOfAttendeesIsGreaterThanSeatsOfTableReserved);
    }

    [Fact]
    public void Request_Should_ReturnAReservationInstance_WhenSuccessful()
    {
        //Necessary information from restaurant for implementing robust business logic 

        var restaurantTests = new RestaurantTests();

        var restaurant = restaurantTests.CreateRestaurant();

        var availableTables = restaurant.RestaurantTables
            .Where(r => !r.ReservedHours
                .Any(
                    t => t.ReservationDateTime.Date == _reservationInformation.ReservationDateTime.Date &&
                         t.ReservationTimeRange.Start <= _reservationInformation.ReservationDateTime.TimeOfDay &&
                         t.ReservationTimeRange.End > _reservationInformation.ReservationDateTime.TimeOfDay))
            .Select(r => r.Number)
            .ToList();

        var numberOfSeats = restaurant.RestaurantTables
                    .Where(g => g.Number == _reservationInformation.ReservedTable)
                    .Select(g => g.Seats)
                    .SingleOrDefault();

        var reservation = Reservation.Request(_reservationInformation,
            numberOfSeats,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        bool isReservationSuccessful = reservation
            .Value
            .GetType() == typeof(Reservation);

        Assert.True(isReservationSuccessful);
    }

    [Fact]
    public void Request_Should_RaiseAReservationRequestedDomainEvent_WhenSuccessful()
    {
        var restaurantTests = new RestaurantTests();

        var restaurant = restaurantTests.CreateRestaurant();

        var availableTables = restaurant.RestaurantTables
            .Where(r => !r.ReservedHours
                .Any(
                    t => t.ReservationDateTime.Date == _reservationInformation.ReservationDateTime.Date &&
                         t.ReservationTimeRange.Start <= _reservationInformation.ReservationDateTime.TimeOfDay &&
                         t.ReservationTimeRange.End > _reservationInformation.ReservationDateTime.TimeOfDay))
            .Select(r => r.Number)
            .ToList();

        var numberOfSeats = restaurant.RestaurantTables
                    .Where(g => g.Number == _reservationInformation.ReservedTable)
                    .Select(g => g.Seats)
                    .SingleOrDefault();

        var reservation = Reservation.Request(_reservationInformation,
            numberOfSeats,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        bool isReservationSuccessful = reservation
            .Value
            .DomainEvents
            .Any(r => r.GetType() == typeof(ReservationRequestedDomainEvent));

        Assert.True(isReservationSuccessful);
    }

    [Fact]
    public void Cancel_Should_ReturnAnError_WhenReservationHasBeenAssisted()
    {
        var reservation = Reservation.Request(_reservationInformation,
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        //Assist reservation
        reservation.Value.Visit(reservation.Value.ReservationAttendees.ClientId);

        var cancel = reservation.Value.Cancel(reservation.Value.ReservationAttendees.ClientId);

        bool isErrorCancelWhenReservationStatusIsAssisted = cancel
            .FirstError
            .Code == "Reservation.CancelWhenReservationStatusIsAssisted";

        Assert.True(isErrorCancelWhenReservationStatusIsAssisted);
    }

    [Fact]
    public void Visit_Should_ReturnAnError_WhenReservationStatusIsNotRequested()
    {
        ReservationInformation reservationInformation = ReservationInformation.Create(
            reservedTable: 1,
            DateTime.Now.TimeOfDay,
            DateTime.Now.AddHours(2).AddMinutes(45).TimeOfDay,
            DateTime.Now);

        var reservation = Reservation.Request(reservationInformation,
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        reservation.Value.Cancel(reservation.Value.ReservationAttendees.ClientId);

        var visit = reservation.Value.Visit(reservation.Value.ReservationAttendees.ClientId);

        bool isErrorAssistWhenReservationStatusIsNotPaid = visit
            .FirstError
            .Code == "Reservation.AssistWhenReservationStatusIsNotPaid";

        Assert.True(isErrorAssistWhenReservationStatusIsNotPaid);
    }

    [Fact]
    public void Visit_Should_RaiseReservationVisitedDomainEvent_WhenSuccessful()
    {
        ReservationInformation reservationInformation = ReservationInformation.Create(
            reservedTable: 1,
            DateTime.Now.TimeOfDay,
            DateTime.Now.AddHours(2).AddMinutes(45).TimeOfDay,
            DateTime.Now);

        var reservation = Reservation.Request(reservationInformation,
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        reservation.Value.Visit(reservation.Value.ReservationAttendees.ClientId);

        bool hasRaisedReservationVisitedDomainEvent = reservation
            .Value
            .DomainEvents
            .Any(r => r.GetType() == typeof(ReservationVisitedDomainEvent));

        Assert.True(hasRaisedReservationVisitedDomainEvent);
    }

    [Fact]
    public void Visit_Should_TurnReservationStatusToVisiting_WhenSuccessful()
    {
        ReservationInformation reservationInformation = ReservationInformation.Create(
            reservedTable: 1,
            DateTime.Now.TimeOfDay,
            DateTime.Now.AddHours(2).AddMinutes(45).TimeOfDay,
            DateTime.Now);

        var reservation = Reservation.Request(reservationInformation,
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        reservation.Value.Visit(reservation.Value.ReservationAttendees.ClientId);

        bool hasRaisedReservationVisitedDomainEvent = reservation
            .Value
            .ReservationStatus == ReservationStatus.Visiting;

        Assert.True(hasRaisedReservationVisitedDomainEvent);
    }

    [Fact]
    public void Finish_Should_ReturnAnError_WhenReservationStatusIsNotVisiting()
    {
        var reservation = Reservation.Request(_reservationInformation,
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        var finish = reservation.Value.Finish(reservation.Value.ReservationAttendees.ClientId);

        bool isErrorCannotFinishIfStatusIsNotAssisting = finish
            .FirstError
            .Code == "Reservation.CannotFinishIfStatusIsNotAssisting";

        Assert.True(isErrorCannotFinishIfStatusIsNotAssisting);
    }

    [Fact]
    public void Finish_Should_RaiseAReservationFinishedDomainEvent_WhenSuccessful()
    {
        ReservationInformation reservationInformation = ReservationInformation.Create(
            reservedTable: 1,
            DateTime.Now.TimeOfDay,
            DateTime.Now.AddHours(2).AddMinutes(45).TimeOfDay,
            DateTime.Now);

        var reservation = Reservation.Request(reservationInformation,
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        reservation.Value.Visit(reservation.Value.ReservationAttendees.ClientId);

        reservation.Value.Finish(reservation.Value.ReservationAttendees.ClientId);

        bool hasRaisedReservationFinishedDomainEvent = reservation
            .Value
            .DomainEvents
            .Any(r => r.GetType() == typeof(ReservationFinishedDomainEvent));

        Assert.True(hasRaisedReservationFinishedDomainEvent);
    }

    [Fact]
    public void Finish_Should_TurnReservationStatusToFinished_WhenSuccessful()
    {
        ReservationInformation reservationInformation = ReservationInformation.Create(
            reservedTable: 1,
            DateTime.Now.TimeOfDay,
            DateTime.Now.AddHours(2).AddMinutes(45).TimeOfDay,
            DateTime.Now);

        var reservation = Reservation.Request(reservationInformation,
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        reservation.Value.Visit(reservation.Value.ReservationAttendees.ClientId);

        reservation.Value.Finish(reservation.Value.ReservationAttendees.ClientId);

        bool isReservationStatusFinished = reservation
            .Value
            .ReservationStatus == ReservationStatus.Finished;

        Assert.True(isReservationStatusFinished);
    }

    [Fact]
    public void UpdateAttendees_Should_ReturnAnError_WhenNumberOfAttendeesIsGreaterThanSeatsOfTableReserved()
    {
        var reservation = Reservation.Request(_reservationInformation,
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        var updateAttendees = reservation.Value.UpdateAttendees(numberOfAttendees: 5, numberOfSeats: 4);

        bool isErrorCannotReserveWhenNumberOfAttendeesIsGreaterThanSeatsOfTableReserved = updateAttendees
            .FirstError
            .Code == "Reservation.CannotReserveWhenNumberOfAttendeesIsGreaterThanSeatsOfTableReserved";

        Assert.True(isErrorCannotReserveWhenNumberOfAttendeesIsGreaterThanSeatsOfTableReserved);
    }

    [Fact]
    public void DeleteMenu_Should_ReturnAnError_WhenMenuDoesNotExistWithinTheReservationMenus()
    {
        var reservation = Reservation.Request(_reservationInformation,
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        var deleteMenu = reservation.Value.DeleteMenu(MenuId.CreateUnique());

        bool isErrorMenuDoesNotExist = deleteMenu.FirstError.Code == "Reservation.MenuNotFound";

        Assert.True(isErrorMenuDoesNotExist);
    }
}
