using Dinners.Application.Common;
using Dinners.Domain.Reservations;
using Dinners.Domain.Reservations.Errors;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Domain.Restaurants;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Reservations.Cancel;

internal sealed class CancelReservationCommandHandler : ICommandHandler<CancelReservationCommand, ErrorOr<Unit>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRestaurantRepository _restaurantRepository;

    public CancelReservationCommandHandler(IReservationRepository reservationRepository, IRestaurantRepository restaurantRepository)
    {
        _reservationRepository = reservationRepository;
        _restaurantRepository = restaurantRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        Reservation? reservation = await _reservationRepository.GetByIdAsync(ReservationId.Create(request.ReservationId), cancellationToken);

        if (reservation is null)
        {
            return ReservationErrorsCodes.NotFound;
        }

        var cancelReservation = reservation.Cancel();

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

        return Unit.Value;
    }
}
