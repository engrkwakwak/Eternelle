using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

/// <summary>
/// Owned entity within the Wedding aggregate. Represents the snap-share section
/// configuration — the Instagram handle to tag and the call-to-action shown to guests.
///
/// Never instantiated outside the Wedding aggregate — use Wedding.ConfigureSnapShare().
/// Never has its own repository — always loaded as part of the Wedding aggregate.
/// </summary>
public sealed class SnapShareConfig : Entity
{
    private SnapShareConfig()
    {
    }

    public SnapShareConfigId Id { get; private set; }

    public WeddingId WeddingId { get; private set; }

    /// <summary>
    /// The Instagram account guests are prompted to tag in their photos.
    /// Stored without the '@' — use InstagramHandle.ToDisplayString() when rendering.
    /// </summary>
    public InstagramHandle? InstagramHandle { get; private set; }

    /// <summary>
    /// Optional call-to-action text shown to guests, e.g. "Tag us @carlandvina2026 in your photos!"
    /// Free-form text — length validated at the application layer.
    /// </summary>
    public string? CtaText { get; private set; }

    public bool Enabled { get; private set; }

    internal static SnapShareConfig Create(
        WeddingId weddingId,
        InstagramHandle? instagramHandle,
        string? ctaText,
        bool enabled)
    {
        return new SnapShareConfig
        {
            Id = SnapShareConfigId.New(),
            WeddingId = weddingId,
            InstagramHandle = instagramHandle,
            CtaText = ctaText,
            Enabled = enabled
        };
    }

    internal void Update(InstagramHandle? instagramHandle, string? ctaText, bool enabled)
    {
        InstagramHandle = instagramHandle;
        CtaText = ctaText;
        Enabled = enabled;
    }
}
