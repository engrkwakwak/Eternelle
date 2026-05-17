using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Shared;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.VendorCredits;

/// <summary>
/// A vendor acknowledgment displayed in the credits section (wedding.vendor_credits).
///
/// VendorCredit is its own aggregate root — it has an independent lifecycle and is
/// always referenced by WeddingId, never navigated through the Wedding aggregate.
///
/// instagram_handle reuses the InstagramHandle value object from the Weddings namespace.
/// The domain stores the normalized handle (without '@'); use ToDisplayString() when
/// rendering to guests.
///
/// display_order is managed explicitly: Create() receives the intended position,
/// and Reorder() handles moves.
/// </summary>
public sealed class VendorCredit : Entity
{
    private VendorCredit()
    {
    }

    public VendorCreditId Id { get; private set; }

    /// <summary>
    /// Cross-aggregate reference to the parent wedding.
    /// Never navigated — use IVendorCreditRepository.GetByWeddingIdAsync() to query.
    /// </summary>
    public WeddingId WeddingId { get; private set; }

    public VendorName Name { get; private set; }

    /// <summary>
    /// The vendor's role (e.g. "Photography", "Catering", "Flowers").
    /// Free text — no closed vocabulary; couples define their own labels.
    /// </summary>
    public PersonRole Role { get; private set; }

    /// <summary>
    /// Optional vendor website URL. The domain stores, not validates, URLs.
    /// </summary>
    public WebUrl? WebsiteUrl { get; private set; }

    public ImageUrl? ImageUrl { get; private set; }

    /// <summary>
    /// Optional Instagram handle for the vendor. Reuses the InstagramHandle value object
    /// from the Weddings namespace — normalized without '@', same validation rules.
    /// </summary>
    public InstagramHandle? InstagramHandle { get; private set; }

    public int DisplayOrder { get; private set; }

    // ─── Factory ────────────────────────────────────────────────────────────────

    public static VendorCredit Create(
        WeddingId weddingId,
        VendorName name,
        PersonRole role,
        WebUrl? websiteUrl,
        ImageUrl? imageUrl,
        InstagramHandle? instagramHandle,
        int displayOrder)
    {
        var credit = new VendorCredit
        {
            Id = VendorCreditId.New(),
            WeddingId = weddingId,
            Name = name,
            Role = role,
            WebsiteUrl = websiteUrl,
            ImageUrl = imageUrl,
            InstagramHandle = instagramHandle,
            DisplayOrder = displayOrder
        };

        credit.Raise(new VendorCreditCreatedDomainEvent(credit.Id, weddingId));

        return credit;
    }

    // ─── Behaviour ──────────────────────────────────────────────────────────────

    public void Update(
        VendorName name,
        PersonRole role,
        WebUrl? websiteUrl,
        ImageUrl? imageUrl,
        InstagramHandle? instagramHandle)
    {
        Name = name;
        Role = role;
        WebsiteUrl = websiteUrl;
        ImageUrl = imageUrl;
        InstagramHandle = instagramHandle;
    }

    public void Reorder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }
}
