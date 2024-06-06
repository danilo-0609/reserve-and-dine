using BuildingBlocks.Domain.AggregateRoots;
using Dinners.Domain.Menus;
using Dinners.Domain.Reservations.DomainEvents;
using Dinners.Domain.Reservations.Errors;
using Dinners.Domain.Reservations.Rules;
using Dinners.Domain.Restaurants;
using ErrorOr;

namespace Dinners.Domain.Reservations;

public sealed class Reservation : AggregateRoot<ReservationId, Guid>
{
    private readonly List<MenuId> _menuIds = new();

    public new ReservationId Id { get; private set; }

    public ReservationInformation ReservationInformation { get; private set; }

    public List<MenuId> MenuIds => _menuIds;

    public RestaurantId RestaurantId { get; private set; }

    public ReservationAttendees ReservationAttendees { get; private set; }

    public ReservationStatus ReservationStatus { get; private set; }

    public DateTime RequestedAt { get; private set; }

    public DateTime? CancelledAt { get; private set; }

    public static ErrorOr<Reservation> Request(ReservationInformation reservationInformation,
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

    public ErrorOr<Success> Cancel(string causeOfCancellation = "")
    {
        var canCancelReservation = CheckRule(new CannotCancelWhenReservationStatusIsNotRequestedRule(ReservationStatus));

        if (canCancelReservation.IsError)
        {
            return canCancelReservation.FirstError;
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

        return new Success();
    }

    public ErrorOr<Success> Visit()
    {
        var mustVisitInReservationTime = CheckRule(new MustAssistToReservationInTheRequestedTimeRule(DateTime.Now, ReservationInformation.ReservationDateTime));
        
        if (mustVisitInReservationTime.IsError)
        {
            return mustVisitInReservationTime.FirstError;
        }

        var cannotVisitWhenReservationIsCancelled = CheckRule(new CannotVisitWhenReservationStatusIsNotRequestedRule(ReservationStatus));
        
        if (cannotVisitWhenReservationIsCancelled.IsError)
        {
            return cannotVisitWhenReservationIsCancelled.FirstError;
        }

        AddDomainEvent(new ReservationVisitedDomainEvent(Guid.NewGuid(),
            Id,
            RestaurantId,
            ReservationAttendees.ClientId,
            DateTime.UtcNow));

        ReservationStatus = ReservationStatus.Visiting;

        return new Success();
    }

    public ErrorOr<Success> Finish()
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

        ReservationStatus = ReservationStatus.Finished;

        return new Success();
    }

    public Reservation Update(
        ReservationInformation reservationInformation,
        List<MenuId> menuIds,
        ReservationStatus reservationStatus,
        ReservationAttendees reservationAttendees)
    {
        return new Reservation(Id,
            reservationInformation,
            menuIds,
            RestaurantId,
            reservationStatus,
            reservationAttendees,
            RequestedAt,
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
        DateTime? cancelledAt = null)
    {
        Id = id;
        ReservationInformation = reservationInformation;
        RestaurantId = restaurantId;
        ReservationAttendees = reservationAttendees;
        ReservationStatus = reservationStatus;

        RequestedAt = requestedAt;
        CancelledAt = cancelledAt;

        _menuIds = menuIds;
    }

    private Reservation(ReservationId id,
        ReservationInformation reservationInformation,
        List<MenuId> menuIds,
        RestaurantId restaurantId,
        ReservationStatus reservationStatus,
        ReservationAttendees reservationAttendees,
        DateTime requestedAt,
        DateTime? cancelledAt = null)
    {
        Id = id;
        ReservationInformation = reservationInformation;
        _menuIds = menuIds;
        RestaurantId = restaurantId;
        ReservationAttendees = reservationAttendees;
        ReservationStatus = reservationStatus;
        
        RequestedAt = requestedAt;
        CancelledAt = cancelledAt;
    }

    private Reservation() { }
}
