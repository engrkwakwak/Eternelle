using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Shared;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.GalleryImages;

/// <summary>
/// A single image in the wedding photo gallery (wedding.gallery_images).
///
/// GalleryImage is its own aggregate root — it has an independent lifecycle and is
/// always referenced by WeddingId, never navigated through the Wedding aggregate.
///
/// width_px and height_px are optional — the domain stores layout hints for the
/// masonry/grid renderer; they are not always available at upload time.
///
/// display_order is managed explicitly: Create() receives the intended position,
/// and Reorder() handles moves.
/// </summary>
public sealed class GalleryImage : Entity
{
    private GalleryImage()
    {
    }

    public GalleryImageId Id { get; private set; }

    /// <summary>
    /// Cross-aggregate reference to the parent wedding.
    /// Never navigated — use IGalleryImageRepository.GetByWeddingIdAsync() to query.
    /// </summary>
    public WeddingId WeddingId { get; private set; }

    /// <summary>
    /// CDN or storage URL of the image.
    /// </summary>
    public ImageUrl SrcUrl { get; private set; }

    /// <summary>
    /// Accessible alt text for the image. Required — guests using screen readers
    /// should always receive a description.
    /// </summary>
    public AccessibilityText AltText { get; private set; }

    /// <summary>
    /// Original pixel width. Nullable — not always available at upload time.
    /// Stored as a layout hint for the masonry/grid renderer.
    /// </summary>
    public int? WidthPx { get; private set; }

    /// <summary>
    /// Original pixel height. Nullable — same reasoning as WidthPx.
    /// </summary>
    public int? HeightPx { get; private set; }

    public ImageCaption? Caption { get; private set; }

    public int DisplayOrder { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    // ─── Factory ────────────────────────────────────────────────────────────────

    public static GalleryImage Create(
        WeddingId weddingId,
        ImageUrl srcUrl,
        AccessibilityText altText,
        int? widthPx,
        int? heightPx,
        ImageCaption? caption,
        int displayOrder,
        DateTime utcNow)
    {
        var image = new GalleryImage
        {
            Id = GalleryImageId.New(),
            WeddingId = weddingId,
            SrcUrl = srcUrl,
            AltText = altText,
            WidthPx = widthPx,
            HeightPx = heightPx,
            Caption = caption,
            DisplayOrder = displayOrder,
            CreatedAtUtc = utcNow
        };

        image.Raise(new GalleryImageCreatedDomainEvent(image.Id, weddingId));

        return image;
    }

    // ─── Behaviour ──────────────────────────────────────────────────────────────

    public void Update(
        ImageUrl srcUrl,
        AccessibilityText altText,
        int? widthPx,
        int? heightPx,
        ImageCaption? caption)
    {
        SrcUrl = srcUrl;
        AltText = altText;
        WidthPx = widthPx;
        HeightPx = heightPx;
        Caption = caption;
    }

    public void Reorder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }
}
