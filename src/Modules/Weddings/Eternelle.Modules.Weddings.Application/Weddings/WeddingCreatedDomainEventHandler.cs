using Eternelle.Common.Application.EventBus;
using Eternelle.Common.Application.Messaging;
using Eternelle.Modules.Weddings.Domain.Weddings;
using Eternelle.Modules.Weddings.IntegrationEvents.Weddings;

namespace Eternelle.Modules.Weddings.Application.Weddings;

internal sealed class WeddingCreatedDomainEventHandler(
    IWeddingRepository weddingRepository,
    IEventBus eventBus)
    : DomainEventHandler<WeddingCreatedDomainEvent>
{
    public override async Task Handle(WeddingCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Wedding? wedding = await weddingRepository.GetAsync(domainEvent.WeddingId, cancellationToken);

        if (wedding is null)
        {
            return;
        }

        await eventBus.PublishAsync(
            new WeddingCreatedIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredOnUtc,
                wedding.Id.Value,
                wedding.TenantId,
                wedding.WeddingDate),
            cancellationToken);
    }
}
