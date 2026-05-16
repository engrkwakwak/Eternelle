using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Shared;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

/// <summary>
/// A single person in the wedding entourage (wedding.entourage_members).
///
/// EntourageMember is an owned entity — it has no independent lifecycle and is
/// always created, updated, and deleted through the EntourageGroup aggregate root.
///
/// Every member must belong to a group — there is no ungrouped member concept.
/// wedding_id is denormalized on the row (mirrors the schema) to allow direct
/// queries by WeddingId without loading the group.
///
/// seed is an optional integer used to deterministically generate an avatar
/// placeholder when image_url is null — keeps placeholder rendering stable
/// across renders without storing an image.
/// </summary>
public sealed class EntourageMember : Entity
{
    private EntourageMember()
    {
    }

    public EntourageMemberId Id { get; private set; }

    /// <summary>
    /// Denormalized cross-aggregate reference. Mirrors wedding.entourage_members.wedding_id.
    /// Never navigated — queries by WeddingId go through IEntourageGroupRepository.
    /// </summary>
    public WeddingId WeddingId { get; private set; }

    public EntourageGroupId GroupId { get; private set; }

    public PersonName Name { get; private set; }

    /// <summary>
    /// Role within this group (e.g. "Ninong", "Best Man", "Maid of Honor").
    /// Free text — no fixed vocabulary; the group type provides semantic meaning.
    /// </summary>
    public PersonRole Role { get; private set; }

    public ImageUrl? ImageUrl { get; private set; }

    /// <summary>
    /// Optional message from this member (e.g. a short congratulatory note).
    /// </summary>
    public PersonMessage? Message { get; private set; }

    /// <summary>
    /// Optional internal note visible only to the couple (e.g. "Needs dietary reminder").
    /// Not rendered to guests.
    /// </summary>
    public InternalNote? Note { get; private set; }

    /// <summary>
    /// Seed for deterministic generative avatar placeholder when ImageUrl is null.
    /// The domain stores, not generates, the seed value.
    /// </summary>
    public int? Seed { get; private set; }

    public int DisplayOrder { get; private set; }

    // ─── Factory ────────────────────────────────────────────────────────────────

    internal static EntourageMember Create(
        WeddingId weddingId,
        EntourageGroupId groupId,
        PersonName name,
        PersonRole role,
        ImageUrl? imageUrl,
        PersonMessage? message,
        InternalNote? note,
        int? seed,
        int displayOrder)
    {
        return new EntourageMember
        {
            Id = EntourageMemberId.New(),
            WeddingId = weddingId,
            GroupId = groupId,
            Name = name,
            Role = role,
            ImageUrl = imageUrl,
            Message = message,
            Note = note,
            Seed = seed,
            DisplayOrder = displayOrder
        };
    }

    // ─── Behaviour ──────────────────────────────────────────────────────────────

    internal void Update(
        PersonName name,
        PersonRole role,
        ImageUrl? imageUrl,
        PersonMessage? message,
        InternalNote? note,
        int? seed)
    {
        Name = name;
        Role = role;
        ImageUrl = imageUrl;
        Message = message;
        Note = note;
        Seed = seed;
    }

    internal void Reorder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }
}
