using Dinners.Domain.Menus;
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

    public async Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken)
    {
        await _dbContext
            .Reservations
            .ExecuteUpdateAsync(x =>
                x.SetProperty(r => r.Id, reservation.Id)
                 .SetProperty(r => r.ReservationInformation, reservation.ReservationInformation)
                 .SetProperty(r => r.MenuIds, reservation.MenuIds)
                 .SetProperty(r => r.RestaurantId, reservation.RestaurantId)
                 .SetProperty(r => r.ReservationAttendees, reservation.ReservationAttendees)
                 .SetProperty(r => r.ReservationStatus, reservation.ReservationStatus)
                 .SetProperty(r => r.ReservationPaymentId, reservation.ReservationPaymentId)
                 .SetProperty(r => r.RefundId, reservation.RefundId)
                 .SetProperty(r => r.RequestedAt, reservation.RequestedAt)
                 .SetProperty(r => r.PayedAt, reservation.PayedAt)
                 .SetProperty(r => r.CancelledAt, reservation.CancelledAt));
    }
}
