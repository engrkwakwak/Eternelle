namespace Eternelle.Modules.Weddings.Application.GuestPhotos;

public sealed class GuestPhotoResponse
{
    public Guid Id { get; init; }
    public string SrcUrl { get; init; } = string.Empty;
    public string? ThumbnailUrl { get; init; }
    public int? WidthPx { get; init; }
    public int? HeightPx { get; init; }
    public string? UploaderName { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime UploadedAt { get; init; }
    public DateTime? ReviewedAt { get; init; }
}
