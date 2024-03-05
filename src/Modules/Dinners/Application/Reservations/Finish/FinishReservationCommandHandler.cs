using Dinners.Application.Common;
using Dinners.Domain.Reservations.Errors;
using Dinners.Domain.Reservations;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Reservations.Finish;

internal sealed class FinishReservationCommandHandler : ICommandHandler<FinishReservationCommand, ErrorOr<Unit>>
{
    private readonly IReservationRepository _reservationRepository;

    public FinishReservationCommandHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(FinishReservationCommand request, CancellationToken cancellationToken)
    {
        Reservation? reservation = await _reservationRepository.GetByIdAsync(ReservationId.Create(request.ReservationId), cancellationToken);

        if (reservation is null)
        {
            return ReservationErrorsCodes.NotFound;
        }

        ErrorOr<ReservationStatus> finishReservation = reservation.Finish();

        if (finishReservation.IsError)
        {
            return finishReservation.FirstError;
        }

        var reservationUpdate = reservation.Update(reservation.ReservationInformation,
            reservation.MenuIds.ToList(),
            finishReservation.Value,
            reservation.ReservationAttendees,
            reservation.ReservationPaymentId,
            reservation.RefundId);

        await _reservationRepository.UpdateAsync(reservationUpdate, cancellationToken);

        return Unit.Value;
    }
}
