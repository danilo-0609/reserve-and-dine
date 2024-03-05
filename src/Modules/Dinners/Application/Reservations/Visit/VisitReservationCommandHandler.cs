using Dinners.Application.Common;
using Dinners.Domain.Reservations;
using Dinners.Domain.Reservations.Errors;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Reservations.Visit;

internal sealed class VisitReservationCommandHandler : ICommandHandler<VisitReservationCommand, ErrorOr<Unit>>
{
    private readonly IReservationRepository _reservationRepository;

    public VisitReservationCommandHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(VisitReservationCommand request, CancellationToken cancellationToken)
    {
        Reservation? reservation = await _reservationRepository.GetByIdAsync(ReservationId.Create(request.ReservationId), cancellationToken);
    
        if (reservation is null)
        {
            return ReservationErrorsCodes.NotFound;
        }

        ErrorOr<ReservationStatus> asisting = reservation.Visit();
    
        if (asisting.IsError)
        {
            return asisting.FirstError;
        }

        var reservationUpdate = reservation.Update(reservation.ReservationInformation,
            reservation.MenuIds.ToList(),
            asisting.Value,
            reservation.ReservationAttendees,
            reservation.ReservationPaymentId!,
            reservation.RefundId);

        await _reservationRepository.UpdateAsync(reservationUpdate, cancellationToken);

        return Unit.Value;
    }
}
