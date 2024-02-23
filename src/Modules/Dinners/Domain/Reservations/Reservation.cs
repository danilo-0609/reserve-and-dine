using BuildingBlocks.Domain.AggregateRoots;
using Dinners.Domain.Menus;
using Dinners.Domain.Restaurants;

namespace Dinners.Domain.Reservations;

public sealed class Reservation : AggregateRoot<ReservationId, Guid>
{
    public new ReservationId Id { get; private set; }

    public ReservationInformation ReservationInformation { get; private set; }

    public MenuId? MenuId { get; private set; }

    public RestaurantId RestaurantId { get; private set; }

    public Guid CustomerId { get; private set; }

    public ReservationStatus ReservationStatus { get; private set; }

    public DateTime ReservatedAt { get; private set; }

    public DateTime? CancelledAt { get; private set; }

    //Request(); 

    //Cancel();

    //Pay();

    //Confirm();

    //Postpone();

    public Reservation(ReservationId id,
        ReservationInformation reservationInformation,
        MenuId? menuId,
        RestaurantId restaurantId,
        ReservationStatus reservationStatus,
        Guid customerId,
        DateTime reservatedAt,
        DateTime? cancelledAt)
    {
        Id = id;
        ReservationInformation = reservationInformation;
        MenuId = menuId;
        RestaurantId = restaurantId;
        ReservatedAt = reservatedAt;
        CancelledAt = cancelledAt;
        CustomerId = customerId;
        ReservationStatus = reservationStatus;
    }
}
