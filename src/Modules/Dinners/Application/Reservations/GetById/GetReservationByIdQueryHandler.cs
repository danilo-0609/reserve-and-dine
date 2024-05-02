using Dinners.Application.Common;
using Dinners.Domain.Reservations;
using Dinners.Domain.Reservations.Errors;
using ErrorOr;

namespace Dinners.Application.Reservations.GetById;

internal sealed class GetReservationByIdQueryHandler : IQueryHandler<GetReservationByIdQuery, ErrorOr<ReservationResponse>>
{
    private readonly IReservationRepository _reservationRepository;

    public GetReservationByIdQueryHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<ErrorOr<ReservationResponse>> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        Reservation? reservation = await _reservationRepository.GetByIdAsync(ReservationId.Create(request.ReservationId), cancellationToken);
    
        if (reservation is null)
        {
            return ReservationErrorsCodes.NotFound;
        }

        ReservationInformationResponse reservationInformation = new(reservation.ReservationInformation.ReservedTable,
            reservation.ReservationInformation.ReservationPrice,
            reservation.ReservationInformation.TimeOfReservation,
            reservation.ReservationInformation.ReservationDateTime);

        ReservationAttendeesResponse reservationAttendees = new(reservation.ReservationAttendees.ClientId,
            reservation.ReservationAttendees.Name,
            reservation.ReservationAttendees.NumberOfAttendees);

        ReservationResponse reservationResponse = new(reservation.Id.Value,
            reservationInformation,
            reservation.RestaurantId.Value,
            reservationAttendees,
            reservation.ReservationStatus.Value,
            reservation.ReservationPaymentId?.Value,
            reservation.MenuIds.ConvertAll(r => r.Value));

        return reservationResponse;
    }
}
