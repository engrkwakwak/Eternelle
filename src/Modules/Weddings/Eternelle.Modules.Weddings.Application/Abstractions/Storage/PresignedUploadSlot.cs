namespace Eternelle.Modules.Weddings.Application.Abstractions.Storage;

/// <summary>
/// A server-generated upload slot returned to the client before a CDN upload.
/// The client uploads the file to <see cref="PresignedUrl"/> and then redeems
/// <see cref="SlotId"/> in the register call. <see cref="CdnUrl"/> is known at
/// presign time because the server controls the destination key/path.
/// </summary>
public sealed record PresignedUploadSlot(
    Guid SlotId,
    string PresignedUrl,
    string CdnUrl);
