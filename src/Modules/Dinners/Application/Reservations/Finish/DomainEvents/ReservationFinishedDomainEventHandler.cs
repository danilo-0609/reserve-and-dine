using BuildingBlocks.Application;
using Dinners.Domain.Reservations.DomainEvents;
using Dinners.Domain.Restaurants;
using Domain.Restaurants;

namespace Dinners.Application.Reservations.Finish.DomainEvents;

internal sealed class ReservationFinishedDomainEventHandler : IDomainEventHandler<ReservationFinishedDomainEvent>
{
    private readonly IRestaurantRepository _restaurantRepository;

    public ReservationFinishedDomainEventHandler(IRestaurantRepository restaurantRepository)
    {
        _restaurantRepository = restaurantRepository;
    }

    public async Task Handle(ReservationFinishedDomainEvent notification, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(notification.RestaurantId);

        restaurant!.AddRestaurantClient(notification.ClientId);

        var tableFree = restaurant!.FreeTable(notification.NumberOfTable);

        if (tableFree.IsError)
        {
            throw new Exception("Table was not found");
        }

        await _restaurantRepository.UpdateAsync(restaurant);
    }
}
