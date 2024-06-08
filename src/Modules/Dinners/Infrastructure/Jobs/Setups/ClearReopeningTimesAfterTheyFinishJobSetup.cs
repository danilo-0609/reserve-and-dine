using Microsoft.Extensions.Options;
using Quartz;

namespace Dinners.Infrastructure.Jobs.Setups;

internal sealed class ClearReopeningTimesAfterTheyFinishJobSetup : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        var jobKey = new JobKey(nameof(ClearReopeningTimesAfterTheyFinishJob));

        options.AddJob<ClearReopeningTimesAfterTheyFinishJob>(jobBuilder => jobBuilder.WithIdentity(jobKey))
            .AddTrigger(
                trigger =>
                    trigger.ForJob(jobKey)
                    .WithSimpleSchedule(
                        schedule =>
                            schedule.WithIntervalInSeconds(3)
                            .RepeatForever()));
    }
}
