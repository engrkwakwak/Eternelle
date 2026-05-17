using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Shared;

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

    /// <summary>
    /// Subscription tier for this wedding.
    /// Determines guest photo limits and feature access.
    /// Defaults to <see cref="WeddingPlan.Free"/> at creation.
    /// </summary>
    public WeddingPlan Plan { get; private set; }

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
            Plan = WeddingPlan.Free,
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

    // ─── Plan management ─────────────────────────────────────────────────────────

    /// <summary>
    /// Upgrades (or downgrades) the wedding to a new subscription plan.
    /// Called from the Subscriptions module via an integration event handler —
    /// never directly by the user.
    /// </summary>
    public void ChangePlan(WeddingPlan plan, DateTime utcNow)
    {
        WeddingPlan previousPlan = Plan;
        Plan = plan;
        UpdatedAtUtc = utcNow;

        Raise(new WeddingPlanChangedDomainEvent(Id, previousPlan, plan));
    }

    // ─── Partner management ─────────────────────────────────────────────────────

    /// <summary>
    /// Adds a partner to this wedding. At most two partners are allowed (numbers 1 and 2).
    /// The returned partner is tracked by EF Core via the aggregate's navigation property
    /// — callers do not need to persist the partner separately.
    /// </summary>
    public Result<Partner> AddPartner(
        PartnerNumber partnerNumber,
        PersonFirstName firstName,
        PersonLastName lastName,
        Biography? bio,
        ImageUrl? imageUrl,
        DateTime utcNow)
    {
        if (_partners.Any(p => p.PartnerNumber == partnerNumber))
        {
            return Result.Failure<Partner>(WeddingErrors.PartnerAlreadyExists(partnerNumber));
        }

        var partner = Partner.Create(Id, partnerNumber, firstName, lastName, bio, imageUrl);
        _partners.Add(partner);

        UpdatedAtUtc = utcNow;

        Raise(new WeddingPartnerAddedDomainEvent(Id, partner.Id, partnerNumber));

        return partner;
    }

    public Result UpdatePartner(
        PartnerId partnerId,
        PersonFirstName firstName,
        PersonLastName lastName,
        Biography? bio,
        ImageUrl? imageUrl,
        DateTime utcNow)
    {
        Partner? partner = _partners.FirstOrDefault(p => p.Id == partnerId);

        if (partner is null)
        {
            return Result.Failure(WeddingErrors.PartnerNotFound(partnerId));
        }

        partner.Update(firstName, lastName, bio, imageUrl);
        UpdatedAtUtc = utcNow;

        Raise(new WeddingPartnerUpdatedDomainEvent(Id, partnerId));

        return Result.Success();
    }

    // ─── Snap share ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates or updates the snap-share section configuration.
    /// Idempotent — safe to call on first setup and on subsequent edits.
    /// </summary>
    public void ConfigureSnapShare(
        InstagramHandle? instagramHandle,
        CallToAction? callToAction,
        bool enabled,
        SnapShareModerationMode moderationMode,
        bool uploaderNameRequired,
        DateTime utcNow)
    {
        if (SnapShare is null)
        {
            SnapShare = SnapShareConfig.Create(Id, instagramHandle, callToAction, enabled, moderationMode, uploaderNameRequired);
        }
        else
        {
            SnapShare.Update(instagramHandle, callToAction, enabled, moderationMode, uploaderNameRequired);
        }

        UpdatedAtUtc = utcNow;

        Raise(new WeddingSnapShareUpdatedDomainEvent(Id));
    }

    /// <summary>
    /// Rotates the guest photo upload token, invalidating all previously distributed
    /// QR codes. SnapShare must be configured first — returns a failure otherwise.
    /// Raises no domain event (stateless rotate — no downstream consumers need to react).
    /// </summary>
    public Result RegenerateUploadToken(DateTime utcNow)
    {
        if (SnapShare is null)
        {
            return Result.Failure(WeddingErrors.SnapShareNotConfigured);
        }

        SnapShare.RegenerateToken();
        UpdatedAtUtc = utcNow;

        return Result.Success();
    }
}
