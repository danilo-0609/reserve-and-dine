using BuildingBlocks.Application;
using Dinners.Domain.Menus;
using Dinners.Domain.Reservations.DomainEvents;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.RestaurantUsers;
using Domain.Restaurants;

namespace Dinners.Application.Reservations.Asist.DomainEvents;

internal sealed class ReservationAsistedDomainEventHandler : IDomainEventHandler<ReservationAsistedDomainEvent>
{
    private readonly IMenuRepository _menuRepository;
    private readonly IRestaurantRepository _restaurantRepository;

    public ReservationAsistedDomainEventHandler(IMenuRepository menuRepository, IRestaurantRepository restaurantRepository)
    {
        _menuRepository = menuRepository;
        _restaurantRepository = restaurantRepository;
    }

    public async Task Handle(ReservationAsistedDomainEvent notification, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(notification.RestaurantId);

        restaurant!.AddRestaurantClient(notification.ClientId);

        var restaurantUpdate = restaurant.Update(restaurant.NumberOfTables,
            restaurant.AvailableTablesStatus,
            restaurant.RestaurantInformation,
            restaurant.RestaurantLocalization,
            restaurant.RestaurantScheduleStatus,
            restaurant.RestaurantSchedule,
            restaurant.RestaurantContact,
            restaurant.RestaurantRatingIds.ToList(),
            restaurant.RestaurantClients.ToList(),
            restaurant.RestaurantTables.ToList(),
            restaurant.RestaurantAdministrations.ToList());

        await _restaurantRepository.UpdateAsync(restaurantUpdate);

        if (notification.MenuIds.Any())
        {
            ConsumeMenus(notification);
        }
    }

    private void ConsumeMenus(ReservationAsistedDomainEvent notification)
    {
        List<Menu> menus = new();

        notification.MenuIds.ForEach(async id =>
        {
            var menu = await _menuRepository.GetByIdAsync(id!);

            if (menu is not null)
                menus.Add(menu);
        });

        menus.ForEach(menu =>
        {
            MenuConsumer menuConsumer = menu.Consume(notification.ClientId);
        });

        UpdateMenus(menus);
    }

    private void UpdateMenus(List<Menu> menus)
    {
        menus.ForEach(async menu =>
        {
            var menuUpdate = menu.Update(menu.MenuReviewIds.ToList(),
                menu.MenuSpecification,
                menu.DishSpecification,
                menu.MenuSchedule,
                menu.MenuConsumers.ToList(),
                DateTime.UtcNow);

            await _menuRepository.UpdateAsync(menuUpdate);
        });
    }
}
