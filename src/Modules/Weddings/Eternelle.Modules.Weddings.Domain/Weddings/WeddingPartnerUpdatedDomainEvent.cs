using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

public sealed class WeddingPartnerUpdatedDomainEvent : DomainEvent
{
    public WeddingPartnerUpdatedDomainEvent(WeddingId weddingId, PartnerId partnerId)
    {
        WeddingId = weddingId;
        PartnerId = partnerId;
    }

    public WeddingId WeddingId { get; init; }

    public PartnerId PartnerId { get; init; }
}
