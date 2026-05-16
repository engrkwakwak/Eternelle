using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

public sealed class EntourageMemberRemovedDomainEvent : DomainEvent
{
    public EntourageMemberRemovedDomainEvent(EntourageGroupId entourageGroupId, EntourageMemberId entourageMemberId, WeddingId weddingId)
    {
        EntourageGroupId = entourageGroupId;
        EntourageMemberId = entourageMemberId;
        WeddingId = weddingId;
    }

    public EntourageGroupId EntourageGroupId { get; init; }

    public EntourageMemberId EntourageMemberId { get; init; }

    public WeddingId WeddingId { get; init; }
}
