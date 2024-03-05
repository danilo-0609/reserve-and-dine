using Dinners.Application.Common;
using Dinners.Domain.Reservations;
using Dinners.Domain.Reservations.Errors;
using Dinners.Domain.Reservations.ReservationsPayments;
using ErrorOr;

namespace Dinners.Application.Reservations.Payments.Pays;

internal sealed class PayReservationCommandHandler : ICommandHandler<PayReservationCommand, ErrorOr<Guid>>
{
    private readonly IReservationRepository _reservationRepository;

    public PayReservationCommandHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(PayReservationCommand request, CancellationToken cancellationToken)
    {
        Reservation? reservation = await _reservationRepository.GetByIdAsync(ReservationId.Create(request.ReservationId), cancellationToken);

        if (reservation is null)
        {
            return ReservationErrorsCodes.NotFound;
        }

        ErrorOr<ReservationPaymentId> payment = reservation.Pay();

        if (payment.IsError)
        {
            return payment.FirstError;
        }

        await _reservationRepository.UpdateAsync(reservation, cancellationToken);

        return payment.Value.Value;
    }
}
