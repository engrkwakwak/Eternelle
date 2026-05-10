using Eternelle.Common.Application.Messaging;
using Eternelle.Modules.Weddings.Domain.GiftOptions;

namespace Eternelle.Modules.Weddings.Application.GiftOptions;

internal sealed class GiftOptionCreatedDomainEventHandler
    : DomainEventHandler<GiftOptionCreatedDomainEvent>
{
    public override Task Handle(GiftOptionCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
