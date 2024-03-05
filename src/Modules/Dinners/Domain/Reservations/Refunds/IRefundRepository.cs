namespace Dinners.Domain.Reservations.Refunds;

public interface IRefundRepository
{
    Task AddAsync(Refund refund);

    Task<Refund?> GetByIdAsync(RefundId refundId, CancellationToken cancellationToken);
}
