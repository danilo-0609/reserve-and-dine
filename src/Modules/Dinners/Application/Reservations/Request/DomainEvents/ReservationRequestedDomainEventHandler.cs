using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Reservations;
using Dinners.Domain.Reservations.DomainEvents;
using Dinners.Domain.Restaurants;
using Domain.Restaurants;

namespace Dinners.Application.Reservations.Request.DomainEvents;

internal sealed class ReservationRequestedDomainEventHandler : IDomainEventHandler<ReservationRequestedDomainEvent>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReservationRequestedDomainEventHandler(IRestaurantRepository restaurantRepository, IReservationRepository reservationRepository, IUnitOfWork unitOfWork)
    {
        _restaurantRepository = restaurantRepository;
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ReservationRequestedDomainEvent notification, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(notification.RestaurantId);

        var reserveTable = restaurant!.ReserveTable(notification.TableNumber,
            notification.ReservationTimeRange,
            notification.ReservationDateTimeRequested);

        if (reserveTable.IsError)
        {
            await _reservationRepository.DeleteAsync(notification.ReservationId, cancellationToken);

            throw new DomainEventHandlerException(reserveTable.FirstError, DateTime.UtcNow);
        }

        await _restaurantRepository.UpdateAsync(restaurant);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
