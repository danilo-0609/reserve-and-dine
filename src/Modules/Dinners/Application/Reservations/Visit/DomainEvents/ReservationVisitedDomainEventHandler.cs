using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Errors;
using Dinners.Domain.Reservations.DomainEvents;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Domain.Restaurants;
using Microsoft.Extensions.Logging;

namespace Dinners.Application.Reservations.Visit.DomainEvents;

internal sealed class ReservationVisitedDomainEventHandler : IDomainEventHandler<ReservationVisitedDomainEvent>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReservationVisitedDomainEventHandler> _logger;

    public ReservationVisitedDomainEventHandler(IRestaurantRepository restaurantRepository, IUnitOfWork unitOfWork, ILogger<ReservationVisitedDomainEventHandler> logger)
    {
        _restaurantRepository = restaurantRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(ReservationVisitedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Working on {@Name}",
            nameof(ReservationVisitedDomainEventHandler));

        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(notification.RestaurantId);

        if (restaurant is null)
        {
            throw new DomainEventHandlerException(RestaurantErrorCodes.NotFound, DateTime.UtcNow);
        }

        _logger.LogInformation("Still working on {@Name}",
            nameof(ReservationVisitedDomainEventHandler));

        restaurant.AddRestaurantClient(notification.ClientId);

        await _restaurantRepository.UpdateAsync(restaurant);
    }
}
