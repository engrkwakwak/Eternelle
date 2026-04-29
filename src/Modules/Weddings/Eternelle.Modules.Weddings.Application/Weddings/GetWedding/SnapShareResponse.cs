namespace Eternelle.Modules.Weddings.Application.Weddings.GetWedding;

public sealed record SnapShareResponse(
    Guid Id,
    string? InstagramHandle,
    string? CtaText,
    bool Enabled);
