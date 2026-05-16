using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.GiftOptions;

public sealed class GiftOptionCreatedDomainEvent(GiftOptionId giftOptionId, WeddingId weddingId) : DomainEvent
{
    public GiftOptionId GiftOptionId { get; init; } = giftOptionId;

    public WeddingId WeddingId { get; init; } = weddingId;
}
