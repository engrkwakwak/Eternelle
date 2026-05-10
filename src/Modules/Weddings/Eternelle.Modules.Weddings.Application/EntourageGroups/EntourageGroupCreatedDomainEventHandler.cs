using Eternelle.Common.Application.Messaging;
using Eternelle.Modules.Weddings.Domain.EntourageGroups;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups;

internal sealed class EntourageGroupCreatedDomainEventHandler
    : DomainEventHandler<EntourageGroupCreatedDomainEvent>
{
    public override Task Handle(EntourageGroupCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
