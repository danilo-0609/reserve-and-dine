using Dinners.Application.Common;
using Dinners.Domain.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Dinners.Infrastructure.Jobs;

[DisallowConcurrentExecution]
internal sealed class CancelNotAssistedReservationsJob : IJob
{
    private readonly DinnersDbContext _dbContext;
    private readonly ILogger<CancelNotAssistedReservationsJob> _logger;
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelNotAssistedReservationsJob(DinnersDbContext dbContext,
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork,
        ILogger<CancelNotAssistedReservationsJob> logger)
    {
        _dbContext = dbContext;
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    { 
        _logger.LogInformation("Working on: {@Name}. At {@DateTime}",
            nameof(CancelNotAssistedReservationsJob),
            DateTime.UtcNow);

        var expirationLimit = DateTime.UtcNow.AddMinutes(-15);

        List<Reservation> reservations = await _dbContext
            .Reservations
            .Where(t => t.ReservationStatus == ReservationStatus.Requested &&
                    t.ReservationInformation.ReservationDateTime <= expirationLimit)
            .Take(20)
            .ToListAsync();

        foreach (var reservation in reservations)
        {
            reservation.Cancel("Reservation must be assisted up to 15 minutes after reservation date time");

            await _reservationRepository.UpdateAsync(reservation, CancellationToken.None);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
