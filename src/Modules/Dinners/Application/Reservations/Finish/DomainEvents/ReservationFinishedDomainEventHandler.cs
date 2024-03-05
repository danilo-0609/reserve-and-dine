using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Reservations.DomainEvents;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Domain.Restaurants;

namespace Dinners.Application.Reservations.Finish.DomainEvents;

internal sealed class ReservationFinishedDomainEventHandler : IDomainEventHandler<ReservationFinishedDomainEvent>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReservationFinishedDomainEventHandler(IRestaurantRepository restaurantRepository, IUnitOfWork unitOfWork)
    {
        _restaurantRepository = restaurantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ReservationFinishedDomainEvent notification, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(notification.RestaurantId);

        if (restaurant is null)
        {
            throw new DomainEventHandlerException(RestaurantErrorCodes.NotFound, DateTime.UtcNow);
        }

        restaurant.AddRestaurantClient(notification.ClientId);

        var tableFree = restaurant.FreeTable(notification.NumberOfTable);

        if (tableFree.IsError)
        {
            throw new DomainEventHandlerException(tableFree.FirstError, DateTime.UtcNow);
        }

        await _restaurantRepository.UpdateAsync(restaurant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
