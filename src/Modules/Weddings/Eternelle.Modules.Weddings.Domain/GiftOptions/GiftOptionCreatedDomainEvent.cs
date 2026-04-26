using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.GiftOptions;

public sealed class GiftOptionCreatedDomainEvent : DomainEvent
{
    public GiftOptionCreatedDomainEvent(GiftOptionId giftOptionId, WeddingId weddingId)
    {
        GiftOptionId = giftOptionId;
        WeddingId = weddingId;
    }

    public GiftOptionId GiftOptionId { get; init; }

    public WeddingId WeddingId { get; init; }
}
