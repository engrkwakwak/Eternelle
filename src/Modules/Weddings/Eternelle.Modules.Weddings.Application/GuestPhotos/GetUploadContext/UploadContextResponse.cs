namespace Eternelle.Modules.Weddings.Application.GuestPhotos.GetUploadContext;

public sealed record UploadContextResponse(
    Guid WeddingId,
    bool UploaderNameRequired);
