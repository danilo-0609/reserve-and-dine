using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Errors;
using Dinners.Domain.Reservations;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using ErrorOr;

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

        var reservationInformation = ReservationInformation.Create(request.ReservedTable,
            request.Price,
            request.Currency,
            request.StartReservationDateTime.TimeOfDay,
            request.EndReservationDateTime.TimeOfDay,
            request.StartReservationDateTime);

        var reservationAttendees = ReservationAttendees.Create(_executionContextAccessor.UserId,
            request.Name,
            request.NumberOfAttendees);

        if (!restaurant.RestaurantTables.Any(r => r.Number == reservationInformation.ReservedTable))
        {
            return Error.NotFound("Reservation.TableNotFound", "The table was not found");
        }

        List<int> availableTables = restaurant.RestaurantTables
            .Where(r => !r.ReservedHours
                .Any(
                    t => t.ReservationDateTime.Date == reservationInformation.ReservationDateTime.Date &&
                         t.ReservationTimeRange.Start <= reservationInformation.ReservationDateTime &&
                         t.ReservationTimeRange.End > reservationInformation.ReservationDateTime))
            .Select(r => r.Number)
            .ToList();

        var reservation = Reservation.Request(reservationInformation,
            availableTables,   
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
                   new Domain.Common.TimeRange(request.StartReservationDateTime, request.EndReservationDateTime));
        
        if (restaurantTableReservation.IsError)
        {
            return restaurantTableReservation.FirstError;
        }

        await _reservationRepository.AddAsync(reservation.Value, cancellationToken);
        await _restaurantRepository.UpdateAsync(restaurant);

        return reservation.Value.Id.Value;
    }
}
