using Eternelle.Common.Application.Messaging;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.Weddings;

internal sealed class WeddingSnapShareUpdatedDomainEventHandler
    : DomainEventHandler<WeddingSnapShareUpdatedDomainEvent>
{
    public override Task Handle(WeddingSnapShareUpdatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
