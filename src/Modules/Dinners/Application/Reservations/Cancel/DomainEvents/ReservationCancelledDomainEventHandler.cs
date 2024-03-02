using BuildingBlocks.Application;
using Dinners.Domain.Reservations.DomainEvents;
using Dinners.Domain.Restaurants;
using Domain.Restaurants;

namespace Dinners.Application.Reservations.Cancel.DomainEvents;

internal sealed class ReservationCancelledDomainEventHandler : IDomainEventHandler<ReservationCancelledDomainEvent>
{
    private readonly IRestaurantRepository _restaurantRepository;

    public ReservationCancelledDomainEventHandler(IRestaurantRepository restaurantRepository)
    {
        _restaurantRepository = restaurantRepository;
    }

    public async Task Handle(ReservationCancelledDomainEvent notification, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(notification.RestaurantId);

        if (restaurant is null)
        {
            throw new Exception("Restaurant was not found");
        }

        restaurant.CancelReservation(notification.NumberOfTable, notification.ReservationDateTime);

        await _restaurantRepository.UpdateAsync(restaurant);
    }
}
