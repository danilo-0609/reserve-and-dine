using Dinners.Application.Reservations.Cancel;
using Dinners.Domain.Menus;
using Dinners.Domain.Reservations;
using Dinners.Domain.Restaurants;
using Microsoft.EntityFrameworkCore;

namespace Dinners.Tests.IntegrationTests.Reservations;

public sealed class CancelReservationIntegrationTests : BaseIntegrationTest
{
    public CancelReservationIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async void Cancel_Should_ReturnAnError_WhenRestaurantDoesNotExist()
    {
        var command = new CancelReservationCommand(Guid.NewGuid());

        var result = await Sender.Send(command);

        bool isErrorReservationNotFound = result.FirstError.Code == "Reservation.NotFound";

        Assert.True(isErrorReservationNotFound);
    }

    [Fact]
    public async void Cancel_Should_ReturnAnError_WhenReservationStatusIsNotPaidOrRequested()
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

        reservation.Value.Cancel(); //ReservationStatus = Cancelled;

        await DbContext.Reservations.AddAsync(reservation.Value);
        await DbContext.SaveChangesAsync();

        var command = new CancelReservationCommand(reservation.Value.Id.Value);

        var result = await Sender.Send(command);

        bool isErrorAssistWhenReservationStatusIsNotPaid = result
            .FirstError
            .Code == "Reservation.CancelWhenReservationStatusIsNotPaidOrRequested";

        Assert.True(isErrorAssistWhenReservationStatusIsNotPaid);
    }

    [Fact]
    public async void Cancel_Should_TurnReservationStatusToCancelled_WhenSuccessful()
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

        await DbContext.Reservations.AddAsync(reservation.Value);
        await DbContext.SaveChangesAsync();

        var command = new CancelReservationCommand(reservation.Value.Id.Value);

        var result = await Sender.Send(command);

        bool isReservationStatusCancelled = await DbContext
            .Reservations
            .Where(r => r.Id == reservation.Value.Id)
            .AnyAsync(res => res.ReservationStatus == ReservationStatus.Cancelled);

        Assert.True(isReservationStatusCancelled);
    }
}
