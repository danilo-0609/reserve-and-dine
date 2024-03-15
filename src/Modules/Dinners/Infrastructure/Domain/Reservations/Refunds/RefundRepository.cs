using Dinners.Domain.Reservations.Refunds;
using Microsoft.EntityFrameworkCore;

namespace Dinners.Infrastructure.Domain.Reservations.Refunds;

internal sealed class RefundRepository : IRefundRepository
{
    private readonly DinnersDbContext _dbContext;

    public RefundRepository(DinnersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Refund refund)
    {
        await _dbContext.Refunds.AddAsync(refund);
    }

    public async Task<Refund?> GetByIdAsync(RefundId refundId, CancellationToken cancellationToken)
    {
        return await _dbContext.Refunds.Where(r => r.Id == refundId).SingleOrDefaultAsync();
    }
}
