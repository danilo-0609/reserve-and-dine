using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Dinners.Infrastructure.Jobs;

internal sealed class ClearReopeningTimesAfterTheyFinishJob : IJob
{
    private readonly DinnersDbContext _dinnersDbContext;
    private readonly ILogger<ClearReopeningTimesAfterTheyFinishJob> _logger;

    public ClearReopeningTimesAfterTheyFinishJob(DinnersDbContext dinnersDbContext, ILogger<ClearReopeningTimesAfterTheyFinishJob> logger)
    {
        _dinnersDbContext = dinnersDbContext;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Executing job {@Name}, at {@DateTime}",
            nameof(ClearReopeningTimesAfterTheyFinishJob),
            DateTime.Now);

        var restaurants = await _dinnersDbContext
            .Restaurants
            .Include(r => r.RestaurantSchedules)
            .Where(r => r.RestaurantSchedules.Any(r => r.ReopeningTime != null))
            .Take(100)
            .ToListAsync();


        var filteredRestaurants = restaurants
            .Where(t =>
            {
                var currentDaySchedule = t.GetCurrentDaySchedule();
                return t.RestaurantSchedules.Any(r =>
                    r.Day.DayOfWeek == (currentDaySchedule.DayOfWeek) + 1
                    && r.ReopeningTime != null);
            })
            .ToList();

        foreach(var restaurant in filteredRestaurants)
        {
            var currentDaySchedule = restaurant.GetCurrentDaySchedule();

            restaurant
                .RestaurantSchedules
                .Where(r => r.Day.DayOfWeek == currentDaySchedule.DayOfWeek - 1)
                .Single()
                .ClearReopeningTime();
        
            _dinnersDbContext.Restaurants.Update(restaurant);
            await _dinnersDbContext.SaveChangesAsync(); 
        }
    } 
}
