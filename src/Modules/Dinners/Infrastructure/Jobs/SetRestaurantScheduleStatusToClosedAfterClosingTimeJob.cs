using Dinners.Domain.Restaurants.RestaurantSchedules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Dinners.Infrastructure.Jobs;

internal sealed class SetRestaurantScheduleStatusToClosedAfterClosingTimeJob : IJob
{
    private readonly DinnersDbContext _dinnersDbContext;
    private readonly ILogger<SetRestaurantScheduleStatusToClosedAfterClosingTimeJob> _logger;

    public SetRestaurantScheduleStatusToClosedAfterClosingTimeJob(DinnersDbContext dinnersDbContext, ILogger<SetRestaurantScheduleStatusToClosedAfterClosingTimeJob> logger)
    {
        _dinnersDbContext = dinnersDbContext;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Executing {@Name} at {@DateTime}",
            nameof(SetRestaurantScheduleStatusToOpenAfterOpeningTimeJob),
            DateTime.Now);

        var now = DateTime.Now;
        var currentDayOfWeek = now.DayOfWeek;
        var currentTimeOfDay = now.TimeOfDay;

        var oneMinuteAgo = DateTime.Now.AddMinutes(-1);

        var restaurants = await _dinnersDbContext
            .Restaurants
            .Include(r => r.RestaurantSchedules)
            .Where(t => t.RestaurantScheduleStatus == RestaurantScheduleStatus.Open)
            .Take(500)
            .ToListAsync();

        var filteredRestaurants = restaurants
            .Where(t =>
            {
                var currentDaySchedule = t.GetCurrentDaySchedule();
                return t.RestaurantSchedules.Any(r =>
                    r.Day == currentDaySchedule
                    && t.LastCheckedScheduleStatus <= oneMinuteAgo
                    && r.HoursOfOperation.End <= DateTime.Now.TimeOfDay);
            })
            .ToList();

        foreach (var restaurant in filteredRestaurants)
        {
            restaurant.Open(restaurant.RestaurantAdministrations.First().AdministratorId);
            restaurant.LastCheckedScheduleStatus = DateTime.Now;

            _dinnersDbContext.Update(restaurant);
        }

        await _dinnersDbContext.SaveChangesAsync();
    }
}
