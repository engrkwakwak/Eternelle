namespace Eternelle.Modules.Weddings.Application.Weddings.GetWedding;

public sealed record SnapShareResponse(
    Guid SnapShareId,
    string? InstagramHandle,
    string? CtaText,
    bool Enabled);
