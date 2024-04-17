using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.RestaurantSchedules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Dinners.Infrastructure.Jobs;

[DisallowConcurrentExecution]
internal sealed class CancelReservationsAfterRestaurantWasClosedOutOfScheduleJob : IJob
{
    private readonly DinnersDbContext _dbContext;
    private readonly ILogger<CancelReservationsAfterRestaurantWasClosedOutOfScheduleJob> _logger;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelReservationsAfterRestaurantWasClosedOutOfScheduleJob(DinnersDbContext dbContext, 
        ILogger<CancelReservationsAfterRestaurantWasClosedOutOfScheduleJob> logger,
        IRestaurantRepository reservationRepository, 
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _logger = logger;
        _restaurantRepository = reservationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Executing job: {@Name}. At {@DateTime}",
            nameof(CancelReservationsAfterRestaurantWasClosedOutOfScheduleJob),
            DateTime.Now);

        var restaurants = await _dbContext
            .Restaurants
            .Where(r => r.RestaurantScheduleStatus == RestaurantScheduleStatus.Closed)
            .Take(20)
            .ToListAsync();

        foreach (var restaurant in restaurants)
        {
            restaurant.RestaurantTables.ForEach(table =>
            {
                foreach (var reservedHour in table.ReservedHours)
                {
                    if (reservedHour.ReservationDateTime < restaurant
                        .RestaurantSchedules
                        .Where(r => r.Day.DayOfWeek > reservedHour.ReservationDateTime.DayOfWeek)
                        .Single()
                        .HoursOfOperation.Start)
                    {
                        table.CancelReservation(reservedHour.ReservationDateTime);
                    }
                }
            });

            await _restaurantRepository.UpdateAsync(restaurant);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
