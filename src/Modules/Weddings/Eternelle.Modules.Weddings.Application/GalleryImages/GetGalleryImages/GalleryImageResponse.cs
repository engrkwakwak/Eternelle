namespace Eternelle.Modules.Weddings.Application.GalleryImages.GetGalleryImages;

public sealed record GalleryImageResponse(
    Guid Id,
    Guid WeddingId,
    string SrcUrl,
    string AltText,
    int? WidthPx,
    int? HeightPx,
    string? Caption,
    int DisplayOrder,
    DateTime CreatedAtUtc);
