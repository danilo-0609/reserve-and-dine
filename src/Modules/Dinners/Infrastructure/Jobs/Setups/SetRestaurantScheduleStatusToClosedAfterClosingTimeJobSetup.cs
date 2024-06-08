using Microsoft.Extensions.Options;
using Quartz;

namespace Dinners.Infrastructure.Jobs.Setups;

internal sealed class SetRestaurantScheduleStatusToClosedAfterClosingTimeJobSetup : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        var jobKey = new JobKey(nameof(SetRestaurantScheduleStatusToClosedAfterClosingTimeJob));

        options.AddJob<SetRestaurantScheduleStatusToClosedAfterClosingTimeJob>(jobBuilder => jobBuilder.WithIdentity(jobKey))
            .AddTrigger(
                trigger =>
                    trigger.ForJob(jobKey)
                    .WithSimpleSchedule(
                        schedule =>
                            schedule.WithIntervalInSeconds(5)
                            .RepeatForever()));
    }
}
