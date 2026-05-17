using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

public sealed class WeddingCreatedDomainEvent(WeddingId weddingId) : DomainEvent
{
    public WeddingId WeddingId { get; init; } = weddingId;
}
