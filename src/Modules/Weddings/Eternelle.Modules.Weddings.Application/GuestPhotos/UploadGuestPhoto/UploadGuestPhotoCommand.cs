using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.UploadGuestPhoto;

public sealed record UploadGuestPhotoCommand(
    Guid UploadToken,
    string SrcUrl,
    string? ThumbnailUrl,
    string? UploaderName,
    int? WidthPx,
    int? HeightPx) : ICommand<Guid>;
