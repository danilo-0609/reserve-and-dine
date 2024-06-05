using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Errors;
using Dinners.Domain.Reservations;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using ErrorOr;
using TimeRange = Dinners.Domain.Common.TimeRange;

namespace Dinners.Application.Reservations.Request;

internal sealed class RequestReservationCommandHandler : ICommandHandler<RequestReservationCommand, ErrorOr<Guid>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;
    private readonly IMenuRepository _menuRepository;

    public RequestReservationCommandHandler(IReservationRepository reservationRepository, IRestaurantRepository restaurantRepository, IExecutionContextAccessor executionContextAccessor, IMenuRepository menuRepository)
    {
        _reservationRepository = reservationRepository;
        _restaurantRepository = restaurantRepository;
        _executionContextAccessor = executionContextAccessor;
        _menuRepository = menuRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(RequestReservationCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.RestaurantId));

        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        List<MenuId> menuIds = new();

        if (request.MenuIds.Any())
        {
            foreach (var menuId in request.MenuIds)
            {
                if (!await _menuRepository.ExistsAsync(MenuId.Create(menuId), cancellationToken))
                {
                    return MenuErrorCodes.NotFound;
                }

                menuIds.Add(MenuId.Create(menuId));
            }
        }

        if (!restaurant.RestaurantTables.Any(r => r.Number == request.ReservedTable))
        {
            return Error.NotFound("Reservation.TableNotFound", "The table was not found");
        }

        Price price = restaurant
            .RestaurantTables
            .Where(r => r.Number == request.ReservedTable)
            .Select(r => r.Price)
            .Single();

        var startTime = TimeSpan.Parse(request.Start);
        var endTime = TimeSpan.Parse(request.Start);

        var reservationInformation = ReservationInformation.Create(request.ReservedTable,
            price.Amount,
            price.Currency,
            startTime,
            endTime,
            request.ReservationDateTime);

        var reservationAttendees = ReservationAttendees.Create(_executionContextAccessor.UserId,
            request.Name,
            request.NumberOfAttendees);

        var reservation = Reservation.Request(reservationInformation,
            restaurant.RestaurantTables.Where(g => g.Number == reservationInformation.ReservedTable)
                            .Select(g => g.Seats)
                            .SingleOrDefault(),
            RestaurantId.Create(request.RestaurantId),
            reservationAttendees,
            menuIds);

        if (reservation.IsError)
        {
            return reservation.FirstError;
        }

        var restaurantTableReservation = restaurant
            .ReserveTable(request.ReservedTable,
                   new TimeRange(startTime, endTime),
            request.ReservationDateTime);

        if (restaurantTableReservation.IsError)
        {
            return restaurantTableReservation.FirstError;
        }

        await _reservationRepository.AddAsync(reservation.Value, cancellationToken);
        await _restaurantRepository.UpdateAsync(restaurant);
        
        return reservation.Value.Id.Value;
    }
}
