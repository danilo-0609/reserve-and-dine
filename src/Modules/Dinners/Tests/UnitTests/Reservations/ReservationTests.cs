using Dinners.Domain.Menus;
using Dinners.Domain.Reservations;
using Dinners.Domain.Reservations.DomainEvents;
using Dinners.Domain.Restaurants;
using Dinners.Tests.UnitTests.Restaurants;

namespace Dinners.Tests.UnitTests.Reservations;

public sealed class ReservationTests
{
    private readonly ReservationInformation _reservationInformation = ReservationInformation.Create(
        reservedTable: 1,
        25.99m,
        "USD",
        DateTime.Now.AddHours(2).TimeOfDay,
        DateTime.Now.AddHours(2).AddMinutes(45).TimeOfDay,
        DateTime.Now.AddHours(2));

    private readonly ReservationAttendees _reservationAttendees = ReservationAttendees.Create(Guid.NewGuid(),
        "Client name",
        numberOfAttendees: 4);

    [Fact]
    public void Request_Should_ReturnAnError_WhenTableIsNotAvailable()
    {
        var reservation = Reservation.Request(_reservationInformation,
            availableTables: new List<int>() { 2, 3 },
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        bool isErrorCannotBeMadeWhenTableIsNotAvailable = reservation
            .FirstError
            .Code == "Reservation.CannotBeMadeWhenTableIsNotAvailable";

        Assert.True(isErrorCannotBeMadeWhenTableIsNotAvailable);
    }

    [Fact]
    public void Request_Should_ReturnAnError_WhenNumberOfAttendeesIsGreaterThanNumberOfSeats()
    {
        var reservationAttendees = ReservationAttendees.Create(Guid.NewGuid(),
        "Client name",
        numberOfAttendees: 5);

        var reservation = Reservation.Request(_reservationInformation,
            availableTables: new List<int>() { 1, 2, 3 },
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
    public void Request_Should_ReturnAnReservationInstance_WhenSuccessful()
    {
        //Neccessary information from restaurant for implementing robust business logic 

        var restaurantTests = new RestaurantTests();

        var restaurant = restaurantTests.CreateRestaurant();

        var availableTables = restaurant.RestaurantTables
            .Where(r => !r.ReservedHours
                .Any(
                    t => t.ReservationDateTime.Date == _reservationInformation.ReservationDateTime.Date &&
                         t.ReservationTimeRange.Start <= _reservationInformation.ReservationDateTime &&
                         t.ReservationTimeRange.End > _reservationInformation.ReservationDateTime))
            .Select(r => r.Number)
            .ToList();

        var numberOfSeats = restaurant.RestaurantTables
                    .Where(g => g.Number == _reservationInformation.ReservedTable)
                    .Select(g => g.Seats)
                    .SingleOrDefault();

        var reservation = Reservation.Request(_reservationInformation,
            availableTables,
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
                         t.ReservationTimeRange.Start <= _reservationInformation.ReservationDateTime &&
                         t.ReservationTimeRange.End > _reservationInformation.ReservationDateTime))
            .Select(r => r.Number)
            .ToList();

        var numberOfSeats = restaurant.RestaurantTables
                    .Where(g => g.Number == _reservationInformation.ReservedTable)
                    .Select(g => g.Seats)
                    .SingleOrDefault();

        var reservation = Reservation.Request(_reservationInformation,
            availableTables,
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
            new List<int>() { 1, 2, 3 },
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        reservation.Value.Pay();

        //Assist reservation
        reservation.Value.Visit();

        var cancel = reservation.Value.Cancel();

        bool isErrorCancelWhenReservationStatusIsAssisted = cancel
            .FirstError
            .Code == "Reservation.CancelWhenReservationStatusIsAssisted";

        Assert.True(isErrorCancelWhenReservationStatusIsAssisted);
    }

    [Fact]
    public void Cancel_Should_RefundMoneyPaid_WhenReservationHasBeenPaid()
    {
        var reservation = Reservation.Request(_reservationInformation,
            new List<int>() { 1, 2, 3 },
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        reservation.Value.Pay();

        reservation.Value.Cancel();

        bool hasRefundedMoney = reservation
            .Value
            .RefundId is not null;

        Assert.True(hasRefundedMoney);
    }

    [Fact]
    public void Cancel_Should_RaiseReservationCancelledDomainEvent_WhenSuccessful()
    {
        var reservation = Reservation.Request(_reservationInformation,
            new List<int>() { 1, 2, 3 },
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        reservation.Value.Cancel();

        bool hasRaisedReservationCancelledDomainEvent = reservation
            .Value
            .DomainEvents
            .Any(r => r.GetType() == typeof(ReservationCancelledDomainEvent));

        Assert.True(hasRaisedReservationCancelledDomainEvent);
    }

    [Fact]
    public void Pay_Should_ReturnAnError_WhenReservationStatusIsNotRequested()
    {
        var reservation = Reservation.Request(_reservationInformation,
            new List<int>() { 1, 2, 3 },
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        reservation.Value.Cancel();

        var pay = reservation.Value.Pay();

        bool isErrorCannotPayWhenReservationStatusIsNotRequested = pay
            .FirstError
            .Code == "Reservation.CannotPayWhenReservationStatusIsNotRequested";

        Assert.True(isErrorCannotPayWhenReservationStatusIsNotRequested);
    }

    [Fact]
    public void Pay_Should_ReturnAReservationPaymentId_WhenSuccessful()
    {
        var reservation = Reservation.Request(_reservationInformation,
            new List<int>() { 1, 2, 3 },
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        var pay = reservation.Value.Pay();

        bool hasReturnedReservationPaymentId = pay
            .Value is not null;

        Assert.True(hasReturnedReservationPaymentId);
    }

    [Fact]
    public void Visit_Should_ReturnAnError_WhenReservationStatusIsNotPaid()
    {
        var reservation = Reservation.Request(_reservationInformation,
            new List<int>() { 1, 2, 3 },
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        var visit = reservation.Value.Visit();

        bool isErrorAssistWhenReservationStatusIsNotPaid = visit
            .FirstError
            .Code == "Reservation.AssistWhenReservationStatusIsNotPaid";

        Assert.True(isErrorAssistWhenReservationStatusIsNotPaid);
    }

    [Fact]
    public void Visit_Should_RaiseReservationVisitedDomainEvent_WhenSuccessful()
    {
        var reservation = Reservation.Request(_reservationInformation,
            new List<int>() { 1, 2, 3 },
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        reservation.Value.Pay();

        reservation.Value.Visit();

        bool hasRaisedReservationVisitedDomainEvent = reservation
            .Value
            .DomainEvents
            .Any(r => r.GetType() == typeof(ReservationVisitedDomainEvent));

        Assert.True(hasRaisedReservationVisitedDomainEvent);
    }

    [Fact]
    public void Visit_Should_TurnReservationStatusToVisiting_WhenSuccessful()
    {
        var reservation = Reservation.Request(_reservationInformation,
            new List<int>() { 1, 2, 3 },
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        reservation.Value.Pay();

        reservation.Value.Visit();

        bool hasRaisedReservationVisitedDomainEvent = reservation
            .Value
            .ReservationStatus == ReservationStatus.Visiting;

        Assert.True(hasRaisedReservationVisitedDomainEvent);
    }

    [Fact]
    public void Finish_Should_ReturnAnError_WhenReservationStatusIsNotVisiting()
    {
        var reservation = Reservation.Request(_reservationInformation,
            new List<int>() { 1, 2, 3 },
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        var finish = reservation.Value.Finish();

        bool isErrorCannotFinishIfStatusIsNotAsisting = finish
            .FirstError
            .Code == "Reservation.CannotFinishIfStatusIsNotAsisting";

        Assert.True(isErrorCannotFinishIfStatusIsNotAsisting);
    }

    [Fact]
    public void Finish_Should_RaiseAReservationFinishedDomainEvent_WhenSuccessful()
    {
        var reservation = Reservation.Request(_reservationInformation,
            new List<int>() { 1, 2, 3 },
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        reservation.Value.Pay();

        reservation.Value.Visit();

        reservation.Value.Finish();

        bool hasRaisedReservationFinishedDomainEvent = reservation
            .Value
            .DomainEvents
            .Any(r => r.GetType() == typeof(ReservationFinishedDomainEvent));

        Assert.True(hasRaisedReservationFinishedDomainEvent);
    }

    [Fact]
    public void Finish_Should_TurnReservationStatusToFinished_WhenSuccessful()
    {
        var reservation = Reservation.Request(_reservationInformation,
            new List<int>() { 1, 2, 3 },
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        reservation.Value.Pay();

        reservation.Value.Visit();

        reservation.Value.Finish();

        bool isReservationStatusFinished = reservation
            .Value
            .ReservationStatus == ReservationStatus.Finished;

        Assert.True(isReservationStatusFinished);
    }

    [Fact]
    public void UpdateAttendees_Should_ReturnAnError_WhenNumberOfAttendeesIsGreaterThanSeatsOfTableReserved()
    {
        var reservation = Reservation.Request(_reservationInformation,
            new List<int>() { 1, 2, 3 },
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
            new List<int>() { 1, 2, 3 },
            4,
            RestaurantId.CreateUnique(),
            _reservationAttendees,
            new List<MenuId>());

        var deleteMenu = reservation.Value.DeleteMenu(MenuId.CreateUnique());

        bool isErrorMenuDoesNotExist = deleteMenu.FirstError.Code == "Reservation.MenuNotFound";

        Assert.True(isErrorMenuDoesNotExist);
    }
}
