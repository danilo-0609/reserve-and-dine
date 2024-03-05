namespace Dinners.Domain.Reservations.ReservationsPayments;

public interface IReservationPaymentRepository
{
    Task AddAsync(ReservationPayment payment, CancellationToken cancellationToken);

    Task<ReservationPayment?> GetByIdAsync(ReservationPaymentId paymentId, CancellationToken cancellationToken);
}
