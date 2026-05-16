using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.GalleryImages.UpdateGalleryImage;

public sealed record UpdateGalleryImageCommand(
    Guid WeddingId,
    Guid GalleryImageId,
    string SrcUrl,
    string AltText,
    int? WidthPx,
    int? HeightPx,
    string? Caption) : ICommand;
