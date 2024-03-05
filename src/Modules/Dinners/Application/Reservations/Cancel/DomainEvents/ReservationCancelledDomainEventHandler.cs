using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Reservations.DomainEvents;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Domain.Restaurants;

namespace Dinners.Application.Reservations.Cancel.DomainEvents;

internal sealed class ReservationCancelledDomainEventHandler : IDomainEventHandler<ReservationCancelledDomainEvent>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReservationCancelledDomainEventHandler(IRestaurantRepository restaurantRepository, IUnitOfWork unitOfWork)
    {
        _restaurantRepository = restaurantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ReservationCancelledDomainEvent notification, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(notification.RestaurantId);

        if (restaurant is null)
        {
            throw new DomainEventHandlerException(RestaurantErrorCodes.NotFound, DateTime.UtcNow);
        }

        restaurant.CancelReservation(notification.NumberOfTable, notification.ReservationDateTime);

        await _restaurantRepository.UpdateAsync(restaurant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
