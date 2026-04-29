using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

/// <summary>
/// A named group of entourage members (wedding.entourage_groups).
///
/// EntourageGroup is its own aggregate root — it owns:
///   - Members  (wedding.entourage_members — the individual people)
///   - Couples  (wedding.entourage_couples — pairings for rendering ninong/ninang pairs, parents, etc.)
///
/// Business invariants enforced here:
///   - A member can only appear once per couple row (checked at PairMembers time).
///   - Canonical ordering on EntourageCouple is enforced by the owned entity itself —
///     the aggregate root passes IDs in any order and the domain swaps them.
///   - Removing a member also removes all couple rows that reference that member,
///     preventing orphaned couple references.
///   - GroupType is nullable — null means the group is fully custom / user-defined.
///   - PairMembers() is guarded by GroupType: groups whose type never allows couples
///     (FlowerGirls, RingBearers, CoinBearers, BibleReaders, Bridesmaids, Groomsmen)
///     will reject pairing attempts at the domain level.
///
/// display_order on the group is managed explicitly via Reorder().
/// </summary>
public sealed class EntourageGroup : Entity
{
    /// <summary>
    /// Group types that never allow couple pairings within the group.
    /// Bridesmaids and Groomsmen pair across groups (bride's side + groom's side),
    /// not within the same group. The "little ones" group types can pair.
    /// Null (fully custom) groups are always permissive.
    /// </summary>
    private static readonly HashSet<EntourageGroupType> NoCouplesAllowed =
    [
        EntourageGroupType.Bridesmaids,
        EntourageGroupType.Groomsmen,
        EntourageGroupType.FlowerGirls,
        EntourageGroupType.RingBearers,
        EntourageGroupType.CoinBearers,
        EntourageGroupType.BibleReaders
    ];

    private readonly List<EntourageMember> _members = [];
    private readonly List<EntourageCouple> _couples = [];

    private EntourageGroup()
    {
    }

    public static readonly int MaxLabelLength = 150;
    public static readonly int MaxSubtitleLength = 200;

    public EntourageGroupId Id { get; private set; }

    /// <summary>
    /// Cross-aggregate reference to the parent wedding.
    /// Never navigated — use IEntourageGroupRepository.GetByWeddingIdAsync() to query.
    /// </summary>
    public WeddingId WeddingId { get; private set; }

    /// <summary>
    /// Display label for this group (e.g. "Ninongs &amp; Ninangs", "Secondary Sponsors").
    /// Free text — couples often use Filipino-specific labels.
    /// </summary>
    public string Label { get; private set; }

    public string? Subtitle { get; private set; }

    /// <summary>
    /// Semantic type. Null = fully custom / user-defined group.
    /// Used by the renderer to apply type-specific layouts without label-matching.
    /// </summary>
    public EntourageGroupType? GroupType { get; private set; }

    /// <summary>
    /// Controls how the group's members are laid out. Defaults to Cards.
    /// </summary>
    public EntourageRenderMode RenderAs { get; private set; }

    public int DisplayOrder { get; private set; }

    public IReadOnlyCollection<EntourageMember> Members => _members.AsReadOnly();

    public IReadOnlyCollection<EntourageCouple> Couples => _couples.AsReadOnly();

    // ─── Factory ────────────────────────────────────────────────────────────────

    public static EntourageGroup Create(
        WeddingId weddingId,
        string label,
        string? subtitle,
        EntourageGroupType? groupType,
        EntourageRenderMode renderAs,
        int displayOrder)
    {
        var group = new EntourageGroup
        {
            Id = EntourageGroupId.New(),
            WeddingId = weddingId,
            Label = label,
            Subtitle = subtitle,
            GroupType = groupType,
            RenderAs = renderAs,
            DisplayOrder = displayOrder
        };

        group.Raise(new EntourageGroupCreatedDomainEvent(group.Id, weddingId));

        return group;
    }

    // ─── Group details ───────────────────────────────────────────────────────────

    public void UpdateDetails(
        string label,
        string? subtitle,
        EntourageGroupType? groupType,
        EntourageRenderMode renderAs)
    {
        Label = label;
        Subtitle = subtitle;
        GroupType = groupType;
        RenderAs = renderAs;
    }

    public void Reorder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }

    // ─── Member management ───────────────────────────────────────────────────────

    public EntourageMember AddMember(
        string name,
        string role,
        string? imageUrl,
        string? message,
        string? note,
        int? seed,
        int displayOrder)
    {
        var member = EntourageMember.Create(WeddingId, Id, name, role, imageUrl, message, note, seed, displayOrder);
        _members.Add(member);
        return member;
    }

    public Result UpdateMember(
        EntourageMemberId memberId,
        string name,
        string role,
        string? imageUrl,
        string? message,
        string? note,
        int? seed)
    {
        EntourageMember? member = _members.FirstOrDefault(m => m.Id == memberId);

        if (member is null)
        {
            return Result.Failure(EntourageGroupErrors.MemberNotFound(memberId));
        }

        member.Update(name, role, imageUrl, message, note, seed);
        return Result.Success();
    }

    public Result ReorderMember(EntourageMemberId memberId, int displayOrder)
    {
        EntourageMember? member = _members.FirstOrDefault(m => m.Id == memberId);

        if (member is null)
        {
            return Result.Failure(EntourageGroupErrors.MemberNotFound(memberId));
        }

        member.Reorder(displayOrder);
        return Result.Success();
    }

    /// <summary>
    /// Removes the member and all couple rows that reference this member.
    /// This maintains referential integrity within the aggregate boundary —
    /// orphaned couple entries are cleaned up automatically.
    /// </summary>
    public Result RemoveMember(EntourageMemberId memberId)
    {
        EntourageMember? member = _members.FirstOrDefault(m => m.Id == memberId);

        if (member is null)
        {
            return Result.Failure(EntourageGroupErrors.MemberNotFound(memberId));
        }

        // Remove any couple rows that reference this member before removing the member itself.
        _couples.RemoveAll(c => c.MemberAId == memberId || c.MemberBId == memberId);
        _members.Remove(member);

        return Result.Success();
    }

    // ─── Couple management ───────────────────────────────────────────────────────

    /// <summary>
    /// Pairs two members into a couple for rendering. Both members must belong to this
    /// group. Canonical ordering (MemberAId &lt; MemberBId) is enforced by EntourageCouple
    /// — callers pass IDs in any order.
    /// </summary>
    public Result<EntourageCouple> PairMembers(
        EntourageMemberId firstId,
        EntourageMemberId secondId,
        string? note,
        int displayOrder)
    {
        if (GroupType.HasValue && NoCouplesAllowed.Contains(GroupType.Value))
        {
            return Result.Failure<EntourageCouple>(EntourageGroupErrors.CouplesNotAllowed(GroupType.Value));
        }

        if (firstId == secondId)
        {
            return Result.Failure<EntourageCouple>(EntourageGroupErrors.CannotPairMemberWithSelf);
        }

        if (_members.All(m => m.Id != firstId))
        {
            return Result.Failure<EntourageCouple>(EntourageGroupErrors.MemberNotFound(firstId));
        }

        if (_members.All(m => m.Id != secondId))
        {
            return Result.Failure<EntourageCouple>(EntourageGroupErrors.MemberNotFound(secondId));
        }

        // Check for duplicate pair — compare in canonical order.
        (EntourageMemberId canonicalA, EntourageMemberId canonicalB) =
            firstId.Value.CompareTo(secondId.Value) < 0
                ? (firstId, secondId)
                : (secondId, firstId);

        bool pairExists = _couples.Any(c => c.MemberAId == canonicalA && c.MemberBId == canonicalB);

        if (pairExists)
        {
            return Result.Failure<EntourageCouple>(EntourageGroupErrors.CoupleAlreadyExists(firstId, secondId));
        }

        var couple = EntourageCouple.Create(Id, firstId, secondId, note, displayOrder);
        _couples.Add(couple);

        return couple;
    }

    public Result UpdateCouple(EntourageCoupleId coupleId, string? note)
    {
        EntourageCouple? couple = _couples.FirstOrDefault(c => c.Id == coupleId);

        if (couple is null)
        {
            return Result.Failure(EntourageGroupErrors.CoupleNotFound(coupleId));
        }

        couple.Update(note);
        return Result.Success();
    }

    public Result ReorderCouple(EntourageCoupleId coupleId, int displayOrder)
    {
        EntourageCouple? couple = _couples.FirstOrDefault(c => c.Id == coupleId);

        if (couple is null)
        {
            return Result.Failure(EntourageGroupErrors.CoupleNotFound(coupleId));
        }

        couple.Reorder(displayOrder);
        return Result.Success();
    }

    public Result RemoveCouple(EntourageCoupleId coupleId)
    {
        EntourageCouple? couple = _couples.FirstOrDefault(c => c.Id == coupleId);

        if (couple is null)
        {
            return Result.Failure(EntourageGroupErrors.CoupleNotFound(coupleId));
        }

        _couples.Remove(couple);
        return Result.Success();
    }
}
