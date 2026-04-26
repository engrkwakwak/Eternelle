using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

public sealed class WeddingSnapShareUpdatedDomainEvent : DomainEvent
{
    public WeddingSnapShareUpdatedDomainEvent(WeddingId weddingId)
    {
        WeddingId = weddingId;
    }

    public WeddingId WeddingId { get; init; }
}
