using Microsoft.Extensions.Options;
using Quartz;

namespace Dinners.Infrastructure.Outbox.BackgroundJobs;

internal sealed class ProcessDinnersOutboxMessagesJobSetup : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        var jobKey = new JobKey(nameof(ProcessDinnersOutboxMessagesJob));

        options.AddJob<ProcessDinnersOutboxMessagesJob>(jobBuilder => jobBuilder.WithIdentity(jobKey))
            .AddTrigger(
                trigger =>
                    trigger.ForJob(jobKey)
                    .WithSimpleSchedule(
                        schedule =>
                            schedule.WithIntervalInSeconds(10)
                            .RepeatForever()));
    }
}
