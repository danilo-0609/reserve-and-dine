using Microsoft.Extensions.Options;
using Quartz;

namespace Users.Infrastructure.Outbox.Jobs;

internal sealed class ProcessUsersOutboxMessagesJobSetup : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        var jobKey = new JobKey(nameof(ProcessUsersOutboxMessagesJob));

        options.AddJob<ProcessUsersOutboxMessagesJob>(jobBuilder => jobBuilder.WithIdentity(jobKey))
            .AddTrigger(
                trigger =>
                    trigger.ForJob(jobKey)
                    .WithSimpleSchedule(
                        schedule =>
                            schedule.WithIntervalInSeconds(10)
                            .RepeatForever()));
    }
}
