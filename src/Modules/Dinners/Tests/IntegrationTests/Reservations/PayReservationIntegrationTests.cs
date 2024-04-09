using Dinners.Application.Reservations.Payments.Pays;
using Dinners.Domain.Menus;
using Dinners.Domain.Reservations;
using Dinners.Domain.Restaurants;
using Microsoft.EntityFrameworkCore;

namespace Dinners.Tests.IntegrationTests.Reservations;

public sealed class PayReservationIntegrationTests : BaseIntegrationTest
{
    public PayReservationIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async void Pay_Should_ReturnAnError_WhenReservationDoesNotExist()
    {
        var command = new PayReservationCommand(Guid.NewGuid());

        var result = await Sender.Send(command);

        bool isErrorRestaurantNotFound = result.FirstError.Code == "Reservation.NotFound";

        Assert.True(isErrorRestaurantNotFound);
    }

    [Fact]
    public async void Pay_Should_ReturnAnError_WhenReservationStatusIsNotRequested()
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

        reservation.Value.Cancel();

        await DbContext.Reservations.AddAsync(reservation.Value);
        await DbContext.SaveChangesAsync();

        var command = new PayReservationCommand(reservation.Value.Id.Value);

        var result = await Sender.Send(command);

        bool isErrorCannotPayWhenReservationStatusIsNotRequested = result
            .FirstError
            .Code == "Reservation.CannotPayWhenReservationStatusIsNotRequested";

        Assert.True(isErrorCannotPayWhenReservationStatusIsNotRequested);
    }

    [Fact]
    public async void Pay_Should_TurnReservationStatusToPaid_WhenSuccessful()
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

        await DbContext.Reservations.AddAsync(reservation.Value);
        await DbContext.SaveChangesAsync();

        var command = new PayReservationCommand(Guid.NewGuid());

        await Sender.Send(command);
    
        bool isReservationStatusPaid = await DbContext
            .Reservations
            .AnyAsync(r => r.Id == reservation.Value.Id && r.ReservationStatus == ReservationStatus.Paid);
    }

    [Fact]
    public async void Pay_Should_ReturnAPaymentId_WhenSuccessful()
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

        await DbContext.Reservations.AddAsync(reservation.Value);
        await DbContext.SaveChangesAsync();

        var command = new PayReservationCommand(Guid.NewGuid());

        var result = await Sender.Send(command);
    
        Assert.IsType<Guid>(result.Value);
    }

    [Fact]
    public async void Pay_Should_PublishAReservationPaidDomainEventToAddThePaymentToTheDatabase_WhenSuccessful()
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

        await DbContext.Reservations.AddAsync(reservation.Value);
        await DbContext.SaveChangesAsync();

        var command = new PayReservationCommand(reservation.Value.Id.Value);

        await Sender.Send(command);

        await Task.Delay(15_000);

        bool isPaymentEntityStoredInDatabase = await DbContext
            .ReservationPayments
            .AnyAsync();

        Assert.True(isPaymentEntityStoredInDatabase);
    }
}


