using Microsoft.Extensions.Options;
using Quartz;

namespace Users.Infrastructure.Jobs.Setups;

internal sealed class ExpireNotConfirmedUserRegistrationsJobSetup : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        var jobKey = new JobKey(nameof(ExpireNotConfirmedUserRegistrationsJob));

        options.AddJob<ExpireNotConfirmedUserRegistrationsJob>(jobBuilder => jobBuilder.WithIdentity(jobKey))
            .AddTrigger(
                trigger =>
                    trigger.ForJob(jobKey)
                    .WithSimpleSchedule(
                        schedule =>
                            schedule.WithIntervalInSeconds(3)
                            .RepeatForever()));
    }
}
