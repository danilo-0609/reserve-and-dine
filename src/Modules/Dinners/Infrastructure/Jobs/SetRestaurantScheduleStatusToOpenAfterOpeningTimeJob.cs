using Dinners.Domain.Restaurants.RestaurantSchedules;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Dinners.Infrastructure.Jobs;

internal sealed class SetRestaurantScheduleStatusToOpenAfterOpeningTimeJob : IJob
{
    private readonly DinnersDbContext _dinnersDbContext;
    private readonly ILogger<SetRestaurantScheduleStatusToOpenAfterOpeningTimeJob> _logger;

    public SetRestaurantScheduleStatusToOpenAfterOpeningTimeJob(DinnersDbContext dinnersDbContext, ILogger<SetRestaurantScheduleStatusToOpenAfterOpeningTimeJob> logger)
    {
        _dinnersDbContext = dinnersDbContext;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Executing {@Name} at {@DateTime}",
            nameof(SetRestaurantScheduleStatusToOpenAfterOpeningTimeJob),
            DateTime.Now);

        var oneMinuteAgo = DateTime.Now.AddMinutes(-1);

        var restaurants = await _dinnersDbContext
            .Restaurants
            .Include(r => r.RestaurantSchedules)
            .Where(t => t.RestaurantScheduleStatus == RestaurantScheduleStatus.Closed)
            .Take(500)
            .ToListAsync();

        var filteredRestaurants = restaurants
            .Where(t =>
            {
                var currentDaySchedule = t.GetCurrentDaySchedule();
                return t.RestaurantSchedules.Any(r =>
                    r.Day == currentDaySchedule
                    && t.LastCheckedScheduleStatus <= oneMinuteAgo
                    && r.HoursOfOperation.Start >= DateTime.Now.TimeOfDay
                    && t.RestaurantSchedules.Where(r => r.Day == currentDaySchedule).Select(r => r.ReopeningTime) is not null);
            })
            .ToList();

        foreach (var restaurant in filteredRestaurants)
        {
            restaurant.Open(restaurant.RestaurantAdministrations.First().AdministratorId);
            restaurant.LastCheckedScheduleStatus = DateTime.Now;

            _dinnersDbContext.Update(restaurant);
            await _dinnersDbContext.SaveChangesAsync();
        }
    }
}
