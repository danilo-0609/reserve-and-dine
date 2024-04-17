using BuildingBlocks.Application;
using Dinners.Domain.Reservations;
using Dinners.Domain.Restaurants.RestaurantTables.Events;

namespace Dinners.Application.Restaurants.Tables.Events;

internal sealed class ReservationTableCancelledDomainEventHandler : IDomainEventHandler<ReservationTableCancelledDomainEvent>
{
    private readonly IReservationRepository _reservationRepository;

    public ReservationTableCancelledDomainEventHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task Handle(ReservationTableCancelledDomainEvent notification, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByRestaurantIdAndReservationTimeAsync(notification.RestaurantId, notification.ReservationDateTime, cancellationToken);

        if (reservation is null)
        {
            return;
        }

        if (reservation.ReservationStatus != ReservationStatus.Cancelled)
        {
            reservation.Cancel("Restaurant was closed out of schedule and reservation cannot be made when restaurant is closed");
        }
    }
}
