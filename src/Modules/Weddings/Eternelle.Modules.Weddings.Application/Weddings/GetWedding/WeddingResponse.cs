namespace Eternelle.Modules.Weddings.Application.Weddings.GetWedding;

public sealed record WeddingResponse(
    Guid Id,
    Guid TenantId,
    DateOnly WeddingDate,
    string? Hashtag,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    IReadOnlyList<PartnerResponse> Partners,
    SnapShareResponse? SnapShare);
