using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Reservations;
using Dinners.Domain.Reservations.Errors;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Reservations.Cancel;

internal sealed class CancelReservationCommandHandler : ICommandHandler<CancelReservationCommand, ErrorOr<Success>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public CancelReservationCommandHandler(IReservationRepository reservationRepository, IRestaurantRepository restaurantRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _reservationRepository = reservationRepository;
        _restaurantRepository = restaurantRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<Success>> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        Reservation? reservation = await _reservationRepository.GetByIdAsync(ReservationId.Create(request.ReservationId), cancellationToken);

        if (reservation is null)
        {
            return ReservationErrorsCodes.NotFound;
        }

        var cancelReservation = reservation.Cancel(_executionContextAccessor.UserId);

        if (cancelReservation.IsError)
        {
            return cancelReservation.FirstError;
        }

        var restaurant = await _restaurantRepository.GetRestaurantById(reservation.RestaurantId);

        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        restaurant.CancelReservation(reservation.ReservationInformation.ReservedTable, 
            reservation.ReservationInformation.ReservationDateTime);

        await _reservationRepository.UpdateAsync(reservation, cancellationToken);
        await _restaurantRepository.UpdateAsync(restaurant);

        return new Success();
    }
}
