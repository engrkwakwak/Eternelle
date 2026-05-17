using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

public sealed class WeddingPartnerAddedDomainEvent(WeddingId weddingId, PartnerId partnerId, PartnerNumber partnerNumber) : DomainEvent
{
    public WeddingId WeddingId { get; init; } = weddingId;

    public PartnerId PartnerId { get; init; } = partnerId;

    public PartnerNumber PartnerNumber { get; init; } = partnerNumber;
}
