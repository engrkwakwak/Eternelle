using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

public sealed class EntourageMemberAddedDomainEvent(EntourageGroupId entourageGroupId, EntourageMemberId entourageMemberId, WeddingId weddingId) : DomainEvent
{
    public EntourageGroupId EntourageGroupId { get; init; } = entourageGroupId;

    public EntourageMemberId EntourageMemberId { get; init; } = entourageMemberId;

    public WeddingId WeddingId { get; init; } = weddingId;
}
