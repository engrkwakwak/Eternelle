namespace Eternelle.Modules.Weddings.Application.GuestPhotos.GenerateUploadSlots;

public sealed record UploadSlotResponse(
    Guid SlotId,
    string PresignedUrl,
    string CdnUrl);
