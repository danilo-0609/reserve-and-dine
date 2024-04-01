using Dinners.Domain.Reservations;
using Microsoft.EntityFrameworkCore;

namespace Dinners.Infrastructure.Domain.Reservations;

internal sealed class ReservationRepository : IReservationRepository
{
    private readonly DinnersDbContext _dbContext;

    public ReservationRepository(DinnersDbContext dinnersDbContext)
    {
        _dbContext = dinnersDbContext;
    }

    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken)
    {
        await _dbContext
            .Reservations
            .AddAsync(reservation, cancellationToken);
    }

    public async Task DeleteAsync(ReservationId reservationId, CancellationToken cancellationToken)
    {
        await _dbContext
            .Reservations
            .Where(r => r.Id == reservationId)
            .ExecuteDeleteAsync();
    }

    public async Task<Reservation?> GetByIdAsync(ReservationId reservationId, CancellationToken cancellationToken)
    {
        return await _dbContext
            .Reservations
            .Where(r => r.Id == reservationId)
            .SingleOrDefaultAsync();
    }

    public Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken)
    {
        _dbContext.Reservations.Update(reservation);

        return Task.CompletedTask;
    }
}
