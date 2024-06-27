using Dinners.Application.Common;
using Dinners.Domain.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Dinners.Infrastructure.Jobs;

internal sealed class FinishReservationsWhenEndTimeHasComeJob : IJob
{
    private readonly DinnersDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FinishReservationsWhenEndTimeHasComeJob> _logger;

    public FinishReservationsWhenEndTimeHasComeJob(DinnersDbContext dbContext, IUnitOfWork unitOfWork, ILogger<FinishReservationsWhenEndTimeHasComeJob> logger)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Executing job: {@Name}. At {@DateTime}",
            nameof(FinishReservationsWhenEndTimeHasComeJob),
            DateTime.Now);

        var reservations = await _dbContext
            .Reservations
            .Where(r => r.ReservationStatus == ReservationStatus.Visiting &&
                   r.ReservationInformation.TimeOfReservation.End >= DateTime.Now.TimeOfDay)
            .Take(100)
            .ToListAsync();

        foreach (var reservation in reservations)
        {
            var finish = reservation.Finish(reservation.ReservationAttendees.ClientId);

            if (finish.IsError)
            {
                _logger.LogError("Finish reservation failed, in {@Name}. At {DateTime}",
                     nameof(FinishReservationsWhenEndTimeHasComeJob),
                    DateTime.Now);
            }
        }

        _dbContext.Reservations.UpdateRange(reservations);
        await _unitOfWork.SaveChangesAsync();
    }
}
