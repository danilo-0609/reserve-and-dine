﻿using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Reservations;
using Dinners.Domain.Reservations.Errors;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using ErrorOr;

namespace Dinners.Application.Reservations.Visit;

internal sealed class VisitReservationCommandHandler : ICommandHandler<VisitReservationCommand, ErrorOr<Success>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public VisitReservationCommandHandler(IReservationRepository reservationRepository, IRestaurantRepository restaurantRepository, IUnitOfWork unitOfWork, IMenuRepository menuRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _reservationRepository = reservationRepository;
        _restaurantRepository = restaurantRepository;
        _menuRepository = menuRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<Success>> Handle(VisitReservationCommand request, CancellationToken cancellationToken)
    {
        Reservation? reservation = await _reservationRepository.GetByIdAsync(ReservationId.Create(request.ReservationId), cancellationToken);
    
        if (reservation is null)
        {
            return ReservationErrorsCodes.NotFound;
        }

        var assisting = reservation.Visit(_executionContextAccessor.UserId);
    
        if (assisting.IsError)
        {
            return assisting.FirstError;
        }

        var restaurant = await _restaurantRepository.GetRestaurantById(reservation.RestaurantId);

        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        var occupyTable = restaurant.OccupyTable(reservation.ReservationInformation.ReservedTable);

        if (occupyTable.IsError)
        {
            return occupyTable.FirstError;
        }

        if (reservation.MenuIds.Any())
        {
            foreach (var menuId in reservation.MenuIds)
            {
                var menu = await _menuRepository.GetByIdAsync(menuId, cancellationToken);

                menu!.Consume(reservation.ReservationAttendees.ClientId);

                await _menuRepository.UpdateAsync(menu!, cancellationToken);
            }
        }

        await _reservationRepository.UpdateAsync(reservation, cancellationToken);
        await _restaurantRepository.UpdateAsync(restaurant);

        return new Success();
    }
}
