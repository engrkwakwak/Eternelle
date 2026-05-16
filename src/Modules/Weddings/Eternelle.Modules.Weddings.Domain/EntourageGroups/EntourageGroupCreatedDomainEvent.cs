using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

public sealed class EntourageGroupCreatedDomainEvent(EntourageGroupId entourageGroupId, WeddingId weddingId) : DomainEvent
{
    public EntourageGroupId EntourageGroupId { get; init; } = entourageGroupId;

    public WeddingId WeddingId { get; init; } = weddingId;
}
