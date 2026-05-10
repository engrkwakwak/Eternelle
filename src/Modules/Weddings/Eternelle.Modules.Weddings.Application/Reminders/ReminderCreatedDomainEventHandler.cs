using Eternelle.Common.Application.Messaging;
using Eternelle.Modules.Weddings.Domain.Reminders;

namespace Eternelle.Modules.Weddings.Application.Reminders;

internal sealed class ReminderCreatedDomainEventHandler
    : DomainEventHandler<ReminderCreatedDomainEvent>
{
    public override Task Handle(ReminderCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
