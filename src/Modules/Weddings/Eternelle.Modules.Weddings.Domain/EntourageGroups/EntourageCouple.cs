using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Shared;

namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

/// <summary>
/// A pairing of two entourage members for rendering purposes (wedding.entourage_couples).
///
/// EntourageCouple is an owned entity — it has no independent lifecycle and is
/// always created, updated, and deleted through the EntourageGroup aggregate root.
///
/// This is a pure mapping: each person's canonical data (name, image, role, message)
/// lives on EntourageMember. The couple entity only groups two members into a
/// rendered pair. Typical uses: Ninong/Ninang pairs, parents groups.
///
/// Canonical ordering invariant: MemberAId.Value &lt; MemberBId.Value (UUID comparison).
/// This prevents the same pair from being registered twice with swapped slots.
/// The domain enforces this automatically — callers do not need to order the IDs
/// before calling PairMembers(); the aggregate root will swap them if needed.
/// </summary>
public sealed class EntourageCouple : Entity
{
    private EntourageCouple()
    {
    }

    public EntourageCoupleId Id { get; private set; }

    public EntourageGroupId GroupId { get; private set; }

    /// <summary>
    /// The member with the lexicographically smaller UUID. Always &lt; MemberBId.
    /// Determined and enforced by the domain on creation — callers pass IDs in
    /// any order and the domain canonicalizes them.
    /// </summary>
    public EntourageMemberId MemberAId { get; private set; }

    /// <summary>
    /// The member with the lexicographically larger UUID. Always &gt; MemberAId.
    /// </summary>
    public EntourageMemberId MemberBId { get; private set; }

    /// <summary>
    /// Optional annotation shown under the rendered pair (e.g. "Lifetime sponsors ♥").
    /// </summary>
    public InternalNote? Note { get; private set; }

    public int DisplayOrder { get; private set; }

    // ─── Factory ────────────────────────────────────────────────────────────────

    internal static EntourageCouple Create(
        EntourageGroupId groupId,
        EntourageMemberId firstId,
        EntourageMemberId secondId,
        InternalNote? note,
        int displayOrder)
    {
        // Enforce canonical ordering: MemberAId < MemberBId (UUID byte comparison).
        // Swap silently so callers don't need to order the IDs themselves.
        (EntourageMemberId memberAId, EntourageMemberId memberBId) =
            firstId.Value.CompareTo(secondId.Value) < 0
                ? (firstId, secondId)
                : (secondId, firstId);

        return new EntourageCouple
        {
            Id = EntourageCoupleId.New(),
            GroupId = groupId,
            MemberAId = memberAId,
            MemberBId = memberBId,
            Note = note,
            DisplayOrder = displayOrder
        };
    }

    // ─── Behaviour ──────────────────────────────────────────────────────────────

    internal void Update(InternalNote? note)
    {
        Note = note;
    }

    internal void Reorder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }
}
