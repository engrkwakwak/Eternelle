using Eternelle.Common.Application.EventBus;
using Eternelle.Common.Application.Messaging;
using Eternelle.Modules.Weddings.Domain.Weddings;
using Eternelle.Modules.Weddings.IntegrationEvents.Weddings;

namespace Eternelle.Modules.Weddings.Application.Weddings;

internal sealed class WeddingCreatedDomainEventHandler(IEventBus eventBus)
    : DomainEventHandler<WeddingCreatedDomainEvent>
{
    public override async Task Handle(WeddingCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await eventBus.PublishAsync(
            new WeddingCreatedIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredOnUtc,
                domainEvent.WeddingId.Value,
                domainEvent.TenantId,
                domainEvent.WeddingDate),
            cancellationToken);
    }
}
