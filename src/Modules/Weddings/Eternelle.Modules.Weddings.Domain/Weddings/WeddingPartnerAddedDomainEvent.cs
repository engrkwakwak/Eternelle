using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

public sealed class WeddingPartnerAddedDomainEvent : DomainEvent
{
    public WeddingPartnerAddedDomainEvent(WeddingId weddingId, PartnerId partnerId, PartnerNumber partnerNumber)
    {
        WeddingId = weddingId;
        PartnerId = partnerId;
        PartnerNumber = partnerNumber;
    }

    public WeddingId WeddingId { get; init; }

    public PartnerId PartnerId { get; init; }

    public PartnerNumber PartnerNumber { get; init; }
}
