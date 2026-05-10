using Eternelle.Common.Application.Messaging;
using Eternelle.Modules.Weddings.Domain.CeremonyActs;

namespace Eternelle.Modules.Weddings.Application.CeremonyActs;

internal sealed class CeremonyActCreatedDomainEventHandler
    : DomainEventHandler<CeremonyActCreatedDomainEvent>
{
    public override Task Handle(CeremonyActCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
