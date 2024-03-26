using Dinners.Application.Common;
using Dinners.Domain.Reservations;
using Dinners.Domain.Reservations.Errors;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Reservations.Visit;

internal sealed class VisitReservationCommandHandler : ICommandHandler<VisitReservationCommand, ErrorOr<Unit>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRestaurantRepository _restaurantRepository;

    public VisitReservationCommandHandler(IReservationRepository reservationRepository, IRestaurantRepository restaurantRepository)
    {
        _reservationRepository = reservationRepository;
        _restaurantRepository = restaurantRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(VisitReservationCommand request, CancellationToken cancellationToken)
    {
        Reservation? reservation = await _reservationRepository.GetByIdAsync(ReservationId.Create(request.ReservationId), cancellationToken);
    
        if (reservation is null)
        {
            return ReservationErrorsCodes.NotFound;
        }

        var asisting = reservation.Visit();
    
        if (asisting.IsError)
        {
            return asisting.FirstError;
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

        var reservationUpdate = reservation.Update(reservation.ReservationInformation,
            reservation.MenuIds.ToList(),
            reservation.ReservationStatus,
            reservation.ReservationAttendees,
            reservation.ReservationPaymentId!,
            reservation.RefundId);

        await _reservationRepository.UpdateAsync(reservationUpdate, cancellationToken);
        await _restaurantRepository.UpdateAsync(restaurant);

        return Unit.Value;
    }
}
