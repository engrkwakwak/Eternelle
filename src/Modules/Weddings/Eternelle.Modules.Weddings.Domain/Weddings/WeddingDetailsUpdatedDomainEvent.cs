using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

public sealed class WeddingDetailsUpdatedDomainEvent : DomainEvent
{
    public WeddingDetailsUpdatedDomainEvent(WeddingId weddingId)
    {
        WeddingId = weddingId;
    }

    public WeddingId WeddingId { get; init; }
}
