using Dinners.Application.Common;
using Dinners.Domain.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Dinners.Infrastructure.Jobs;

[DisallowConcurrentExecution]
internal sealed class CancelNotAsistedReservationsJob : IJob
{
    private readonly DinnersDbContext _dbContext;
    private readonly ILogger<CancelNotPaidReservationsJob> _logger;
    private readonly IReservationRepository _reservationRepository;
    private readonly CancellationToken _cancellationToken;
    private readonly IUnitOfWork _unitOfWork;

    public CancelNotAsistedReservationsJob(DinnersDbContext dbContext, 
        ILogger<CancelNotPaidReservationsJob> logger, 
        IReservationRepository reservationRepository, 
        CancellationToken cancellationToken, 
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _logger = logger;
        _reservationRepository = reservationRepository;
        _cancellationToken = cancellationToken;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Working on: {@Name}. At {@DateTime}",
            nameof(CancelNotPaidReservationsJob),
            DateTime.UtcNow);

        var expirationLimit = DateTime.UtcNow.AddMinutes(-15);

        List<Reservation> reservations = await _dbContext
            .Reservations
            .Where(t => t.ReservationStatus == ReservationStatus.Payed &&
                    t.ReservationInformation.ReservationDateTime >= expirationLimit)
            .Take(20)
            .ToListAsync();

        foreach (var reservation in reservations)
        {
            reservation.Cancel("Reservation must be assisted up to 15 minutes after reservation date time");

            await _reservationRepository.UpdateAsync(reservation, _cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(_cancellationToken);
    }
}
