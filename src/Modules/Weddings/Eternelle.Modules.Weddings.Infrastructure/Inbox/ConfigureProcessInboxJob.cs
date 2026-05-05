using Microsoft.Extensions.Options;
using Quartz;

namespace Eternelle.Modules.Weddings.Infrastructure.Inbox;

internal sealed class ConfigureProcessInboxJob(IOptions<InboxOptions> inboxOptions)
    : IConfigureOptions<QuartzOptions>
{
    private readonly InboxOptions _inboxOptions = inboxOptions.Value;

    public void Configure(QuartzOptions options)
    {
        if (_inboxOptions.IntervalInSeconds <= 0)
        {
            throw new InvalidOperationException(
                $"Weddings:Inbox:IntervalInSeconds must be a positive integer. Current value: {_inboxOptions.IntervalInSeconds}");
        }

        string jobName = typeof(ProcessInboxJob).FullName!;

        options
            .AddJob<ProcessInboxJob>(configure => configure.WithIdentity(jobName))
            .AddTrigger(configure =>
                configure
                    .ForJob(jobName)
                    .WithSimpleSchedule(schedule =>
                        schedule.WithIntervalInSeconds(_inboxOptions.IntervalInSeconds).RepeatForever()));
    }
}
