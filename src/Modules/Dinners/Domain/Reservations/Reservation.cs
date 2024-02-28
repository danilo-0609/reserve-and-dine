using BuildingBlocks.Domain.AggregateRoots;
using Dinners.Domain.Menus;
using Dinners.Domain.Reservations.Devolutions;
using Dinners.Domain.Reservations.DomainEvents;
using Dinners.Domain.Reservations.Errors;
using Dinners.Domain.Reservations.ReservationsPayments;
using Dinners.Domain.Reservations.Rules;
using Dinners.Domain.Restaurants;
using ErrorOr;

namespace Dinners.Domain.Reservations;

public sealed class Reservation : AggregateRoot<ReservationId, Guid>
{
    private readonly List<MenuId?> _menuIds = new();

    public new ReservationId Id { get; private set; }

    public ReservationInformation ReservationInformation { get; private set; }

    public IReadOnlyList<MenuId?> MenuIds => _menuIds.AsReadOnly();

    public RestaurantId RestaurantId { get; private set; }

    public ReservationAttendees ReservationAttendees { get; private set; }

    public ReservationStatus ReservationStatus { get; private set; }

    public ReservationPaymentId? ReservationPaymentId { get; private set; }


    public static ErrorOr<Reservation> Request(ReservationInformation reservationInformation,
        List<int> availableTables,
        int numberOfSeats,
        RestaurantId restaurantId,
        ReservationAttendees reservationAttendees,
        List<MenuId?> menuIds)
    {
        Reservation reservation = new Reservation(ReservationId.CreateUnique(),
            reservationInformation,
            restaurantId,
            reservationAttendees,
            ReservationStatus.Requested,
            menuIds);

        var isTableReservedNow = reservation.CheckRule(new ReservationCannotBeMadeWhenTableIsNotAvailableRule(availableTables, reservationInformation.ReservedTable));

        if (isTableReservedNow.IsError)
        {
            return isTableReservedNow.FirstError;
        }

        var isnumberOfAttendeesGreaterThanTableSeats = reservation.CheckRule(
            new CannotReservedWhenNumberOfAttendeesIsGreaterThanSeatsOfTableReservedRule(reservationAttendees.NumberOfAttendess, numberOfSeats));

        if (isnumberOfAttendeesGreaterThanTableSeats.IsError)
        {
            return isnumberOfAttendeesGreaterThanTableSeats.FirstError;
        }

        reservation.AddDomainEvent(new ReservationRequestedDomainEvent(
            Guid.NewGuid(),
            reservation.Id,
            reservation.ReservationAttendees.ClientId,
            DateTime.UtcNow));

        return reservation;
    }

    public ErrorOr<ReservationStatus> Cancel()
    {
        var canCancelReservation = CheckRule(new CannotCancelWhenReservationStatusIsNotPayedOrRequesteddRule(ReservationStatus));

        if (canCancelReservation.IsError)
        {
            return canCancelReservation.FirstError;
        }

        if (ReservationStatus == ReservationStatus.Payed)
        {
            Devolution.Refund(
                Id,
                ReservationAttendees.ClientId,
                ReservationInformation.ReservationPrice,
                DateTime.UtcNow);

            return ReservationStatus.Cancelled;
        }

        AddDomainEvent(new ReservationCancelledDomainEvent(
            Guid.NewGuid(),
            Id,
            DateTime.UtcNow));

        return ReservationStatus.Cancelled;
    }

    public ErrorOr<ReservationStatus> Pay()
    {
        var payment = ReservationPayment.PayFromReservation(
            ReservationAttendees.ClientId,
            Id,
            ReservationInformation.ReservationPrice,
            ReservationStatus,
            DateTime.UtcNow);

        if (payment.IsError)
        {
            return payment.FirstError;
        }

        AddDomainEvent(new ReservationCancelledDomainEvent(Guid.NewGuid(),
            Id,
            DateTime.UtcNow));

        return ReservationStatus.Payed;
    }

    public ErrorOr<ReservationStatus> Asist()
    {
        var cannotAssistWhenReservationStatusIsNotPayedRule = CheckRule(new CannotAssistWhenReservationStatusIsNotPayedRule(ReservationStatus));

        if (cannotAssistWhenReservationStatusIsNotPayedRule.IsError)
        {
            return cannotAssistWhenReservationStatusIsNotPayedRule.FirstError;
        }

        AddDomainEvent(new ReservationAsistedDomainEvent(Guid.NewGuid(),
            Id,
            _menuIds,
            DateTime.UtcNow));

        return ReservationStatus.Asisted;
    }

    public Reservation Update(ReservationInformation reservationInformation,
        List<MenuId?> menuIds,
        ReservationStatus reservationStatus,
        ReservationAttendees reservationAttendees,
        ReservationPaymentId reservationPaymentId)
    {
        return new Reservation(Id,
            reservationInformation,
            menuIds,
            RestaurantId,
            reservationStatus,
            reservationAttendees,
            reservationPaymentId);
    }

    public ErrorOr<ReservationAttendees> UpdateAttendees(int numberOfAttendees, int numberOfSeats)
    {
        var numberOfAttendeesCannotBeGreaterThanNumberOfSeatsRule = CheckRule(new CannotReservedWhenNumberOfAttendeesIsGreaterThanSeatsOfTableReservedRule(numberOfAttendees, numberOfSeats));

        if (numberOfAttendeesCannotBeGreaterThanNumberOfSeatsRule.IsError)
        {
            return numberOfAttendeesCannotBeGreaterThanNumberOfSeatsRule.FirstError;
        }

        ReservationAttendees.UpdateAttendees(numberOfAttendees);

        return ReservationAttendees;
    }

    public List<MenuId> AddMenu(MenuId menuId)
    {
        _menuIds.Add(menuId);

        return _menuIds!;
    }

    public ErrorOr<List<MenuId>> DeleteMenu(MenuId menuId)
    {
        if (!_menuIds.Contains(menuId))
        {
            return ReservationErrorsCodes.MenuNotFound;
        }

        _menuIds.Remove(menuId);

        return _menuIds!;
    }
    
    private Reservation(ReservationId id,
        ReservationInformation reservationInformation,
        RestaurantId restaurantId,
        ReservationAttendees reservationAttendees,
        ReservationStatus reservationStatus,
        List<MenuId?> menuIds)
    {
        Id = id;
        ReservationInformation = reservationInformation;
        RestaurantId = restaurantId;
        ReservationAttendees = reservationAttendees;
        ReservationStatus = reservationStatus;

        _menuIds = menuIds;
    }

    private Reservation(ReservationId id,
        ReservationInformation reservationInformation,
        List<MenuId?> menuIds,
        RestaurantId restaurantId,
        ReservationStatus reservationStatus,
        ReservationAttendees reservationAttendees,
        ReservationPaymentId reservationPaymentId)
    {
        Id = id;
        ReservationInformation = reservationInformation;
        _menuIds = menuIds;
        RestaurantId = restaurantId;
        ReservationAttendees = reservationAttendees;
        ReservationStatus = reservationStatus;
        ReservationPaymentId = reservationPaymentId;
    }

    private Reservation() { }
}
