namespace Eternelle.Modules.Weddings.Application.GuestPhotos.RegisterGuestPhotos;

/// <summary>
/// Describes one photo to register after the client has uploaded it to the CDN
/// using the presigned URL issued by <c>GenerateUploadSlotsCommand</c>.
/// </summary>
public sealed record PhotoRegistration(
    Guid SlotId,
    string? UploaderName,
    int? WidthPx,
    int? HeightPx);
