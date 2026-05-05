namespace Eternelle.Modules.Weddings.Application.Weddings.GetWedding;

public sealed record WeddingResponse(
    Guid Id,
    Guid TenantId,
    DateOnly WeddingDate,
    string? Hashtag,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc)
{
    public List<PartnerResponse> Partners { get; } = [];
    public SnapShareResponse? SnapShare { get; set; }
}
