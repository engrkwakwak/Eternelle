using Eternelle.Common.Application.Messaging;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.Weddings;

internal sealed class WeddingDetailsUpdatedDomainEventHandler
    : DomainEventHandler<WeddingDetailsUpdatedDomainEvent>
{
    public override Task Handle(WeddingDetailsUpdatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
