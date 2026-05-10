using Eternelle.Common.Application.EventBus;

namespace Eternelle.Modules.Weddings.Application.Weddings;

public sealed class WeddingCreatedIntegrationEvent : IntegrationEvent
{
    public WeddingCreatedIntegrationEvent(Guid id, DateTime occurredOnUtc, Guid weddingId)
        : base(id, occurredOnUtc)
    {
        WeddingId = weddingId;
    }

    public Guid WeddingId { get; init; }
}
