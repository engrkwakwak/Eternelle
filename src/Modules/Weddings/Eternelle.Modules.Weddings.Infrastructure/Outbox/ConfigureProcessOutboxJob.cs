using Microsoft.Extensions.Options;
using Quartz;

namespace Eternelle.Modules.Weddings.Infrastructure.Outbox;

internal sealed class ConfigureProcessOutboxJob(IOptions<OutboxOptions> outboxOptions)
    : IConfigureOptions<QuartzOptions>
{
    private readonly OutboxOptions _outboxOptions = outboxOptions.Value;

    public void Configure(QuartzOptions options)
    {
        if (_outboxOptions.IntervalInSeconds <= 0)
        {
            throw new InvalidOperationException(
                $"Weddings:Outbox:IntervalInSeconds must be a positive integer. Current value: {_outboxOptions.IntervalInSeconds}");
        }

        string jobName = typeof(ProcessOutboxJob).FullName!;

        options
            .AddJob<ProcessOutboxJob>(configure => configure.WithIdentity(jobName))
            .AddTrigger(configure =>
                configure
                    .ForJob(jobName)
                    .WithSimpleSchedule(schedule =>
                        schedule.WithIntervalInSeconds(_outboxOptions.IntervalInSeconds).RepeatForever()));
    }
}
