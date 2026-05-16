using Eternelle.Common.Application.EventBus;

namespace Eternelle.Modules.Weddings.IntegrationEvents.Weddings;

public sealed class WeddingCreatedIntegrationEvent : IntegrationEvent
{
    public WeddingCreatedIntegrationEvent(
        Guid id,
        DateTime occurredOnUtc,
        Guid weddingId,
        Guid tenantId,
        DateOnly weddingDate)
        : base(id, occurredOnUtc)
    {
        WeddingId = weddingId;
        TenantId = tenantId;
        WeddingDate = weddingDate;
    }

    public Guid WeddingId { get; init; }

    public Guid TenantId { get; init; }

    public DateOnly WeddingDate { get; init; }
}
