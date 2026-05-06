using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

/// <summary>
/// Wedding aggregate root. Represents the wedding profile (wedding.profiles table)
/// and directly owns:
///   - Partners   (wedding.partners — always exactly two, partner_number 1 and 2)
///   - SnapShare  (wedding.snap_share_configs — 1:1, no independent lifecycle)
///
/// All other child aggregates — StoryMoment, EntourageGroup, GalleryImage, GiftOption,
/// DressCodeConfig, CeremonyAct, VendorCredit, Reminder — reference this aggregate by
/// WeddingId but are managed as separate aggregate roots with their own repositories.
///
/// Business invariants enforced here:
///   - A wedding can have at most two partners (partner_number 1 and 2).
///   - PartnerNumber must be 1 or 2 — no ungrouped or third-party partners.
///   - TenantId is immutable after creation.
///   - SchemaVersion is bumped only by explicit migration, not by user edits.
/// </summary>
public sealed class Wedding : Entity
{
    private readonly List<Partner> _partners = [];

    private Wedding()
    {
    }

    public WeddingId Id { get; private set; }

    /// <summary>
    /// Cross-module reference to tenancy.tenants. Treated as an opaque identifier —
    /// the Wedding module never joins into the Tenancy module's tables.
    /// </summary>
    public Guid TenantId { get; private set; }

    /// <summary>
    /// Tracks content shape for future migrations. Starts at 1 and is bumped
    /// only when a structural migration script runs — never by user edits.
    /// </summary>
    public int SchemaVersion { get; private set; }

    /// <summary>
    /// Primary wedding date. Used for the countdown, hero headline, and sort.
    /// Individual event dates live on the CeremonyAct / WeddingEvent aggregates.
    /// </summary>
    public DateOnly WeddingDate { get; private set; }

    /// <summary>
    /// Optional wedding hashtag. Stored without the leading '#'.
    /// Use Hashtag.ToDisplayString() when rendering to guests.
    /// </summary>
    public Hashtag? Hashtag { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>
    /// The two partners for this wedding. Always exactly two once fully configured
    /// (partner_number 1 and 2). May be empty or have one entry during initial setup.
    /// </summary>
    public IReadOnlyCollection<Partner> Partners => _partners.AsReadOnly();

    /// <summary>
    /// Snap-share section configuration. Null until ConfigureSnapShare() is first called.
    /// </summary>
    public SnapShareConfig? SnapShare { get; private set; }

    // ─── Factory ────────────────────────────────────────────────────────────────

    public static Wedding Create(
        Guid tenantId,
        DateOnly weddingDate,
        Hashtag? hashtag,
        DateTime utcNow)
    {
        var wedding = new Wedding
        {
            Id = WeddingId.New(),
            TenantId = tenantId,
            SchemaVersion = 1,
            WeddingDate = weddingDate,
            Hashtag = hashtag,
            CreatedAtUtc = utcNow,
            UpdatedAtUtc = utcNow
        };

        wedding.Raise(new WeddingCreatedDomainEvent(wedding.Id));

        return wedding;
    }

    // ─── Core details ────────────────────────────────────────────────────────────

    public Result UpdateDetails(
        DateOnly weddingDate,
        Hashtag? hashtag,
        DateTime utcNow)
    {
        WeddingDate = weddingDate;
        Hashtag = hashtag;
        UpdatedAtUtc = utcNow;

        Raise(new WeddingDetailsUpdatedDomainEvent(Id));

        return Result.Success();
    }

    // ─── Partner management ─────────────────────────────────────────────────────

    /// <summary>
    /// Adds a partner to this wedding. At most two partners are allowed (numbers 1 and 2).
    /// The returned partner is tracked by EF Core via the aggregate's navigation property
    /// — callers do not need to persist the partner separately.
    /// </summary>
    public Result<Partner> AddPartner(
        PartnerNumber partnerNumber,
        string firstName,
        string lastName,
        string? bio,
        string? imageUrl,
        DateTime utcNow)
    {
        if (_partners.Any(p => p.PartnerNumber == partnerNumber))
        {
            return Result.Failure<Partner>(WeddingErrors.PartnerAlreadyExists(partnerNumber));
        }

        var partner = Partner.Create(Id, partnerNumber, firstName, lastName, bio, imageUrl);
        _partners.Add(partner);

        UpdatedAtUtc = utcNow;

        return partner;
    }

    public Result UpdatePartner(
        PartnerId partnerId,
        string firstName,
        string lastName,
        string? bio,
        string? imageUrl,
        DateTime utcNow)
    {
        Partner? partner = _partners.FirstOrDefault(p => p.Id == partnerId);

        if (partner is null)
        {
            return Result.Failure(WeddingErrors.PartnerNotFound(partnerId));
        }

        partner.Update(firstName, lastName, bio, imageUrl);
        UpdatedAtUtc = utcNow;

        return Result.Success();
    }

    // ─── Snap share ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates or updates the snap-share section configuration.
    /// Idempotent — safe to call on first setup and on subsequent edits.
    /// </summary>
    public void ConfigureSnapShare(
        InstagramHandle? instagramHandle,
        string? ctaText,
        bool enabled,
        SnapShareModerationMode moderationMode,
        DateTime utcNow)
    {
        if (SnapShare is null)
        {
            SnapShare = SnapShareConfig.Create(Id, instagramHandle, ctaText, enabled, moderationMode);
        }
        else
        {
            SnapShare.Update(instagramHandle, ctaText, enabled, moderationMode);
        }

        UpdatedAtUtc = utcNow;

        Raise(new WeddingSnapShareUpdatedDomainEvent(Id));
    }

    /// <summary>
    /// Rotates the guest photo upload token, invalidating all previously distributed
    /// QR codes. Safe to call at any time — the new token takes effect immediately.
    /// Raises no domain event (stateless rotate — no downstream consumers need to react).
    /// </summary>
    public void RegenerateUploadToken()
    {
        SnapShare?.RegenerateToken();
    }
}
