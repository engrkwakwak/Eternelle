using Eternelle.Common.Application.Messaging;
using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs;

internal sealed class DressCodeConfigCreatedDomainEventHandler
    : DomainEventHandler<DressCodeConfigCreatedDomainEvent>
{
    public override Task Handle(DressCodeConfigCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
