using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

public sealed class WeddingPartnerUpdatedDomainEvent(WeddingId weddingId, PartnerId partnerId) : DomainEvent
{
    public WeddingId WeddingId { get; init; } = weddingId;

    public PartnerId PartnerId { get; init; } = partnerId;
}
