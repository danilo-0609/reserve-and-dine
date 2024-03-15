using Dinners.Domain.Reservations.ReservationsPayments;
using Microsoft.EntityFrameworkCore;

namespace Dinners.Infrastructure.Domain.Reservations.Payments;

internal sealed class PaymentRepository : IReservationPaymentRepository
{
    private readonly DinnersDbContext _dbContext;

    public PaymentRepository(DinnersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(ReservationPayment payment, CancellationToken cancellationToken)
    {
        await _dbContext.ReservationPayments.AddAsync(payment, cancellationToken);
    }

    public async Task<ReservationPayment?> GetByIdAsync(ReservationPaymentId paymentId, CancellationToken cancellationToken)
    {
        return await _dbContext
            .ReservationPayments
            .Where(r => r.Id == paymentId)
            .SingleOrDefaultAsync();
    }
}
