using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Dinners.Infrastructure.Jobs;

internal sealed class SetCurrentDayScheduleJob : IJob
{
    private readonly DinnersDbContext _dinnersDbContext;
    private readonly ILogger<SetCurrentDayScheduleJob> _logger;

    public SetCurrentDayScheduleJob(DinnersDbContext dinnersDbContext, ILogger<SetCurrentDayScheduleJob> logger)
    {
        _dinnersDbContext = dinnersDbContext;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Executing {@Name} at {@DateTime}",
            nameof(SetCurrentDayScheduleJob),
            DateTime.Now);

        var twoMinutesAgo = DateTime.Now.AddMinutes(-2);

        var restaurants = await _dinnersDbContext
            .Restaurants
            .Where(r => r.LastCheckedCurrentDaySchedule <= twoMinutesAgo)
            .Take(500)
            .ToListAsync();

        foreach(var restaurant in restaurants)
        {
            restaurant.SetCurrentDaySchedule();
            restaurant.LastCheckedCurrentDaySchedule = DateTime.Now;
            _dinnersDbContext.Update(restaurant);
        }

        await _dinnersDbContext.SaveChangesAsync();
    }
}
