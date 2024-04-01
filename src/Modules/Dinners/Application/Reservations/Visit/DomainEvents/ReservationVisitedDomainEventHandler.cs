using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Errors;
using Dinners.Domain.Reservations.DomainEvents;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Domain.Restaurants;

namespace Dinners.Application.Reservations.Visit.DomainEvents;

internal sealed class ReservationVisitedDomainEventHandler : IDomainEventHandler<ReservationVisitedDomainEvent>
{
    private readonly IMenuRepository _menuRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReservationVisitedDomainEventHandler(IMenuRepository menuRepository, IRestaurantRepository restaurantRepository, IUnitOfWork unitOfWork)
    {
        _menuRepository = menuRepository;
        _restaurantRepository = restaurantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ReservationVisitedDomainEvent notification, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(notification.RestaurantId);

        if (restaurant is null)
        {
            throw new DomainEventHandlerException(RestaurantErrorCodes.NotFound, DateTime.UtcNow);
        }

        restaurant.AddRestaurantClient(notification.ClientId);

        await _restaurantRepository.UpdateAsync(restaurant);

        if (notification.MenuIds.Any())
        {
            ConsumeMenus(notification, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private void ConsumeMenus(ReservationVisitedDomainEvent notification, CancellationToken cancellationToken)
    {
        List<Menu> menus = new();

        notification.MenuIds.ForEach(async id =>
        {
            var menu = await _menuRepository.GetByIdAsync(id!, cancellationToken);

            if (menu is null)
            {
                throw new DomainEventHandlerException(MenuErrorCodes.NotFound, DateTime.UtcNow);
            }

            if (menu is not null)
                menus.Add(menu);
        });

        menus.ForEach(menu =>
        {
            MenuConsumer menuConsumer = menu.Consume(notification.ClientId);
        });

        UpdateMenus(menus, cancellationToken);
    }

    private void UpdateMenus(List<Menu> menus, CancellationToken cancellationToken)
    {
        menus.ForEach(async menu =>
        {
            var menuUpdate = menu.Update(menu.MenuReviewIds.ToList(),
                    menu.MenuDetails,
                    menu.DishSpecification,
                    menu.MenuConsumers.ToList(),
                    menu.MenuImagesUrl.ToList(),
                    menu.Tags.ToList(),
                    menu.MenuSchedules.ToList(),
                    menu.Ingredients.ToList(),
                    DateTime.UtcNow);

            await _menuRepository.UpdateAsync(menuUpdate, cancellationToken);
        });
    }
}
