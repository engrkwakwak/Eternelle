using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

public sealed class WeddingCreatedDomainEvent : DomainEvent
{
    public WeddingCreatedDomainEvent(WeddingId weddingId)
    {
        WeddingId = weddingId;
    }

    public WeddingId WeddingId { get; init; }
}
