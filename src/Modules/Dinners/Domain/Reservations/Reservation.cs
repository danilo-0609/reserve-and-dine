using BuildingBlocks.Domain.AggregateRoots;
using Dinners.Domain.Menus;
using Dinners.Domain.Reservations.DomainEvents;
using Dinners.Domain.Reservations.Errors;
using Dinners.Domain.Reservations.Refunds;
using Dinners.Domain.Reservations.ReservationsPayments;
using Dinners.Domain.Reservations.Rules;
using Dinners.Domain.Restaurants;
using ErrorOr;
using MediatR;

namespace Dinners.Domain.Reservations;

public sealed class Reservation : AggregateRoot<ReservationId, Guid>
{
    private readonly List<MenuId> _menuIds = new();

    public new ReservationId Id { get; private set; }

    public ReservationInformation ReservationInformation { get; private set; }

    public IReadOnlyList<MenuId> MenuIds => _menuIds.AsReadOnly();

    public RestaurantId RestaurantId { get; private set; }

    public ReservationAttendees ReservationAttendees { get; private set; }

    public ReservationStatus ReservationStatus { get; private set; }

    public ReservationPaymentId? ReservationPaymentId { get; private set; }

    public RefundId? RefundId { get; private set; }

    public DateTime RequestedAt { get; private set; }

    public DateTime? PaidAt { get; private set; }

    public DateTime? CancelledAt { get; private set; }

    public static ErrorOr<Reservation> Request(ReservationInformation reservationInformation,
        List<int> availableTables,
        int numberOfSeats,
        RestaurantId restaurantId,
        ReservationAttendees reservationAttendees,
        List<MenuId> menuIds)
    {
        Reservation reservation = new Reservation(ReservationId.CreateUnique(),
            reservationInformation,
            restaurantId,
            reservationAttendees,
            ReservationStatus.Requested,
            menuIds,
            DateTime.UtcNow);

        var isTableReservedNow = reservation.CheckRule(new ReservationCannotBeMadeWhenTableIsNotAvailableRule(availableTables, reservationInformation.ReservedTable));

        if (isTableReservedNow.IsError)
        {
            return isTableReservedNow.FirstError;
        }

        var isNumberOfAttendeesGreaterThanTableSeats = reservation.CheckRule(
            new CannotReservedWhenNumberOfAttendeesIsGreaterThanSeatsOfTableReservedRule(reservationAttendees.NumberOfAttendees, numberOfSeats));

        if (isNumberOfAttendeesGreaterThanTableSeats.IsError)
        {
            return isNumberOfAttendeesGreaterThanTableSeats.FirstError;
        }

        reservation.AddDomainEvent(new ReservationRequestedDomainEvent(
            Guid.NewGuid(),
            reservation.Id,
            reservation.RestaurantId,
            reservation.ReservationAttendees.ClientId,
            reservation.ReservationInformation.ReservedTable,
            reservation.ReservationInformation.TimeOfReservation,
            reservation.ReservationInformation.ReservationDateTime,
            DateTime.UtcNow));

        return reservation;
    }

    public ErrorOr<Unit> Cancel(string causeOfCancellation = "")
    {
        var canCancelReservation = CheckRule(new CannotCancelWhenReservationStatusIsNotPayedOrRequesteddRule(ReservationStatus));

        if (canCancelReservation.IsError)
        {
            return canCancelReservation.FirstError;
        }

        if (ReservationStatus == ReservationStatus.Payed)
        {
            Refund refund = Refund.Payback(
                Id,
                ReservationAttendees.ClientId,
                ReservationInformation.ReservationPrice,
                DateTime.UtcNow);

            AddDomainEvent(new ReservationCancelledDomainEvent(
                Guid.NewGuid(),
                Id,
                RestaurantId,
                ReservationInformation.ReservedTable,
                causeOfCancellation,
                ReservationInformation.ReservationDateTime,
                DateTime.UtcNow));

            RefundId = refund.Id;
            CancelledAt = DateTime.UtcNow;
            ReservationStatus = ReservationStatus.Cancelled;

            return Unit.Value;
        }

        AddDomainEvent(new ReservationCancelledDomainEvent(
            Guid.NewGuid(),
            Id,
            RestaurantId,
            ReservationInformation.ReservedTable,
            causeOfCancellation,
            ReservationInformation.ReservationDateTime,
            DateTime.UtcNow));

        CancelledAt = DateTime.UtcNow;
        ReservationStatus = ReservationStatus.Cancelled;

        return Unit.Value;
    }

    public ErrorOr<ReservationPaymentId> Pay()
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

        ReservationStatus = ReservationStatus.Payed;
        PaidAt = DateTime.UtcNow;

        return payment.Value.Id;
    }

    public ErrorOr<ReservationStatus> Visit()
    {
        var cannotVisitWhenReservationStatusIsNotPayedRule = CheckRule(new CannotAssistWhenReservationStatusIsNotPayedRule(ReservationStatus));

        if (cannotVisitWhenReservationStatusIsNotPayedRule.IsError)
        {
            return cannotVisitWhenReservationStatusIsNotPayedRule.FirstError;
        }

        AddDomainEvent(new ReservationVisitedDomainEvent(Guid.NewGuid(),
            Id,
            RestaurantId,
            ReservationAttendees.ClientId,
            _menuIds,
            DateTime.UtcNow));

        return ReservationStatus.Visiting;
    }

    public ErrorOr<ReservationStatus> Finish()
    {
        var statusMustBeVisited = CheckRule(new CannotFinishAReservationWhenReservationStatusIsNotVisitedRule(ReservationStatus));
    
        if (statusMustBeVisited.IsError)
        {
            return statusMustBeVisited.FirstError;
        }

        AddDomainEvent(new ReservationFinishedDomainEvent(Guid.NewGuid(),
            Id,
            RestaurantId,
            ReservationAttendees.ClientId,
            ReservationInformation.ReservedTable,
            DateTime.UtcNow));

        return ReservationStatus.Finished;
    }

    public Reservation Update(ReservationInformation reservationInformation,
        List<MenuId> menuIds,
        ReservationStatus reservationStatus,
        ReservationAttendees reservationAttendees,
        ReservationPaymentId? reservationPaymentId,
        RefundId? refundId)
    {
        return new Reservation(Id,
            reservationInformation,
            menuIds,
            RestaurantId,
            reservationStatus,
            reservationAttendees,
            reservationPaymentId,
            refundId,
            RequestedAt,
            PaidAt,
            CancelledAt);
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
        List<MenuId> menuIds,
        DateTime requestedAt,
        DateTime? payedAt = null,
        DateTime? cancelledAt = null)
    {
        Id = id;
        ReservationInformation = reservationInformation;
        RestaurantId = restaurantId;
        ReservationAttendees = reservationAttendees;
        ReservationStatus = reservationStatus;

        RequestedAt = requestedAt;
        PaidAt = payedAt;
        CancelledAt = cancelledAt;

        _menuIds = menuIds;
    }

    private Reservation(ReservationId id,
        ReservationInformation reservationInformation,
        List<MenuId> menuIds,
        RestaurantId restaurantId,
        ReservationStatus reservationStatus,
        ReservationAttendees reservationAttendees,
        ReservationPaymentId? reservationPaymentId,
        RefundId? refundId,
        DateTime requestedAt,
        DateTime? paidAt = null,
        DateTime? cancelledAt = null)
    {
        Id = id;
        ReservationInformation = reservationInformation;
        _menuIds = menuIds;
        RestaurantId = restaurantId;
        ReservationAttendees = reservationAttendees;
        ReservationStatus = reservationStatus;
        ReservationPaymentId = reservationPaymentId;
        RefundId = refundId;

        RequestedAt = requestedAt;
        PaidAt = paidAt;
        CancelledAt = cancelledAt;
    }

    private Reservation() { }
}
