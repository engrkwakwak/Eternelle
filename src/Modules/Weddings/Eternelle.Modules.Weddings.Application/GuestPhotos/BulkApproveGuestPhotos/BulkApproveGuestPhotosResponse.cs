namespace Eternelle.Modules.Weddings.Application.GuestPhotos.BulkApproveGuestPhotos;

public sealed record BulkApproveGuestPhotosResponse(
    IReadOnlyList<Guid> ApprovedIds,
    IReadOnlyList<Guid> SkippedIds);
