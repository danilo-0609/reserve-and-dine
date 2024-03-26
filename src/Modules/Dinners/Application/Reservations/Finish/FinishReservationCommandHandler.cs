using Dinners.Application.Common;
using Dinners.Domain.Reservations.Errors;
using Dinners.Domain.Reservations;
using ErrorOr;
using MediatR;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;

namespace Dinners.Application.Reservations.Finish;

internal sealed class FinishReservationCommandHandler : ICommandHandler<FinishReservationCommand, ErrorOr<Unit>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRestaurantRepository _restaurantRepository;

    public FinishReservationCommandHandler(IReservationRepository reservationRepository, IRestaurantRepository restaurantRepository)
    {
        _reservationRepository = reservationRepository;
        _restaurantRepository = restaurantRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(FinishReservationCommand request, CancellationToken cancellationToken)
    {
        Reservation? reservation = await _reservationRepository.GetByIdAsync(ReservationId.Create(request.ReservationId), cancellationToken);

        if (reservation is null)
        {
            return ReservationErrorsCodes.NotFound;
        }

        var finishReservation = reservation.Finish();

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

        var reservationUpdate = reservation.Update(reservation.ReservationInformation,
            reservation.MenuIds.ToList(),
            reservation.ReservationStatus,
            reservation.ReservationAttendees,
            reservation.ReservationPaymentId,
            reservation.RefundId);

        await _reservationRepository.UpdateAsync(reservationUpdate, cancellationToken);

        return Unit.Value;
    }
}
