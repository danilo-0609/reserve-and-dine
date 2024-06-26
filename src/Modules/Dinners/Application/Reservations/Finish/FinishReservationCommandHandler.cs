﻿using Dinners.Application.Common;
using Dinners.Domain.Reservations.Errors;
using Dinners.Domain.Reservations;
using ErrorOr;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using BuildingBlocks.Application;

namespace Dinners.Application.Reservations.Finish;

internal sealed class FinishReservationCommandHandler : ICommandHandler<FinishReservationCommand, ErrorOr<Success>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public FinishReservationCommandHandler(IReservationRepository reservationRepository, IRestaurantRepository restaurantRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _reservationRepository = reservationRepository;
        _restaurantRepository = restaurantRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<Success>> Handle(FinishReservationCommand request, CancellationToken cancellationToken)
    {
        Reservation? reservation = await _reservationRepository.GetByIdAsync(ReservationId.Create(request.ReservationId), cancellationToken);

        if (reservation is null)
        {
            return ReservationErrorsCodes.NotFound;
        }

        var finishReservation = reservation.Finish(_executionContextAccessor.UserId);

        if (finishReservation.IsError)
        {
            return finishReservation.FirstError;
        }

        var restaurant = await _restaurantRepository.GetRestaurantById(reservation.RestaurantId);

        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        var freeTable = restaurant.FreeTable(reservation.ReservationInformation.ReservedTable);

        if (freeTable.IsError)
        {
            return freeTable.FirstError;
        }

        await _reservationRepository.UpdateAsync(reservation, cancellationToken);
        await _restaurantRepository.UpdateAsync(restaurant);

        return new Success();
    }
}
