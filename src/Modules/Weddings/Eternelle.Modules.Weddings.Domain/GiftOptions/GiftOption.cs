using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.GiftOptions;

/// <summary>
/// A single gift option card shown in the gift-guide section (wedding.gift_options).
///
/// GiftOption is its own aggregate root — it has an independent lifecycle and is
/// always referenced by WeddingId, never navigated through the Wedding aggregate.
///
/// Business invariant: link_url is required when display_mode = GiftDisplayMode.Link.
/// For ModalDetails and InlineDetails the link_url is unused; the renderer reads
/// qr_image_url and account_* fields instead.
///
/// display_order is managed explicitly: Create() receives the intended position,
/// and Reorder() handles moves.
/// </summary>
public sealed class GiftOption : Entity
{
    private GiftOption()
    {
    }

    public GiftOptionId Id { get; private set; }

    /// <summary>
    /// Cross-aggregate reference to the parent wedding.
    /// Never navigated — use IGiftOptionRepository.GetByWeddingIdAsync() to query.
    /// </summary>
    public WeddingId WeddingId { get; private set; }

    public string Title { get; private set; }

    public string? Description { get; private set; }

    public GiftDisplayMode DisplayMode { get; private set; }

    /// <summary>
    /// Required when DisplayMode = Link. The domain enforces this at Create/Update time.
    /// For ModalDetails and InlineDetails this is null — the QR + account fields are used.
    /// </summary>
    public string? LinkUrl { get; private set; }

    /// <summary>
    /// Card thumbnail image (not the QR code). Optional for all display modes.
    /// </summary>
    public string? ImageUrl { get; private set; }

    /// <summary>
    /// Bank/e-wallet QR code image URL. Relevant for ModalDetails and InlineDetails.
    /// </summary>
    public string? QrImageUrl { get; private set; }

    /// <summary>
    /// Account holder name (e.g. "Carl Marion"). Relevant for ModalDetails and InlineDetails.
    /// </summary>
    public string? AccountName { get; private set; }

    /// <summary>
    /// Account number. Free-form string — formats vary by bank and e-wallet.
    /// </summary>
    public string? AccountNumber { get; private set; }

    /// <summary>
    /// Account type label (e.g. "BPI Savings", "GCash", "Maya", "PayPal").
    /// </summary>
    public string? AccountType { get; private set; }

    public int DisplayOrder { get; private set; }

    // ─── Factory ────────────────────────────────────────────────────────────────

    public static Result<GiftOption> Create(
        WeddingId weddingId,
        string title,
        string? description,
        GiftDisplayMode displayMode,
        string? linkUrl,
        string? imageUrl,
        string? qrImageUrl,
        string? accountName,
        string? accountNumber,
        string? accountType,
        int displayOrder)
    {
        if (displayMode == GiftDisplayMode.Link && string.IsNullOrWhiteSpace(linkUrl))
        {
            return Result.Failure<GiftOption>(GiftOptionErrors.LinkUrlRequired);
        }

        var option = new GiftOption
        {
            Id = GiftOptionId.New(),
            WeddingId = weddingId,
            Title = title,
            Description = description,
            DisplayMode = displayMode,
            LinkUrl = linkUrl,
            ImageUrl = imageUrl,
            QrImageUrl = qrImageUrl,
            AccountName = accountName,
            AccountNumber = accountNumber,
            AccountType = accountType,
            DisplayOrder = displayOrder
        };

        option.Raise(new GiftOptionCreatedDomainEvent(option.Id, weddingId));

        return option;
    }

    // ─── Behaviour ──────────────────────────────────────────────────────────────

    public Result Update(
        string title,
        string? description,
        GiftDisplayMode displayMode,
        string? linkUrl,
        string? imageUrl,
        string? qrImageUrl,
        string? accountName,
        string? accountNumber,
        string? accountType)
    {
        if (displayMode == GiftDisplayMode.Link && string.IsNullOrWhiteSpace(linkUrl))
        {
            return Result.Failure(GiftOptionErrors.LinkUrlRequired);
        }

        Title = title;
        Description = description;
        DisplayMode = displayMode;
        LinkUrl = linkUrl;
        ImageUrl = imageUrl;
        QrImageUrl = qrImageUrl;
        AccountName = accountName;
        AccountNumber = accountNumber;
        AccountType = accountType;

        return Result.Success();
    }

    public void Reorder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }
}
