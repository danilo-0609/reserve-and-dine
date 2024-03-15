using Dinners.Application.Common;
using Dinners.Domain.Reservations;
using Dinners.Domain.Reservations.Errors;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Reservations.Cancel;

internal sealed class CancelReservationCommandHandler : ICommandHandler<CancelReservationCommand, ErrorOr<Unit>>
{
    private readonly IReservationRepository _reservationRepository;

    public CancelReservationCommandHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        Reservation? reservation = await _reservationRepository.GetByIdAsync(ReservationId.Create(request.ReservationId), cancellationToken);

        if (reservation is null)
        {
            return ReservationErrorsCodes.NotFound;
        }

        ErrorOr<Unit> cancelReservation = reservation.Cancel();

        if (cancelReservation.IsError)
        {
            return cancelReservation.FirstError;
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
