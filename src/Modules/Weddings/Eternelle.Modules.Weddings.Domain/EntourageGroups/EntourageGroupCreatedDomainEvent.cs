using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

public sealed class EntourageGroupCreatedDomainEvent : DomainEvent
{
    public EntourageGroupCreatedDomainEvent(EntourageGroupId entourageGroupId, WeddingId weddingId)
    {
        EntourageGroupId = entourageGroupId;
        WeddingId = weddingId;
    }

    public EntourageGroupId EntourageGroupId { get; init; }

    public WeddingId WeddingId { get; init; }
}
