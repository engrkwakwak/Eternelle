using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.GalleryImages.AddGalleryImage;

public sealed record AddGalleryImageCommand(
    Guid WeddingId,
    string SrcUrl,
    string AltText,
    int? WidthPx,
    int? HeightPx,
    string? Caption,
    int DisplayOrder) : ICommand<Guid>;
