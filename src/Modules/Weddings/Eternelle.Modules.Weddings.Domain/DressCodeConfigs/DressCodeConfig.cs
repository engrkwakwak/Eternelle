using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.DressCodeConfigs;

/// <summary>
/// Dress code section configuration for a wedding (wedding.dress_code_configs).
/// 1:1 with wedding.profiles via WeddingId.
///
/// DressCodeConfig is its own aggregate root — it owns:
///   - DressCodeColors  (wedding.dress_code_colors — palette chips)
///   - DressCodeImages  (wedding.dress_code_images — inspiration photos)
///
/// Both child collections are in the same folder as the aggregate root following
/// the owned-entity convention established with Partner and SnapShareConfig.
///
/// Business invariants enforced here:
///   - color_hex values are validated via the HexColor value object, mirroring
///     the DB CHECK constraint ^#[0-9A-Fa-f]{3,8}$.
///   - Callers must already hold the validated HexColor before calling AddColor —
///     the aggregate root accepts HexColor, not raw strings, to keep it free of
///     validation responsibility.
/// </summary>
public sealed class DressCodeConfig : Entity
{
    private readonly List<DressCodeColor> _colors = [];
    private readonly List<DressCodeImage> _images = [];

    private DressCodeConfig()
    {
    }

    public static readonly int MaxDescriptionLength = 2000;

    public DressCodeConfigId Id { get; private set; }

    /// <summary>
    /// Cross-aggregate reference to the parent wedding. 1:1 — enforced by the DB
    /// UNIQUE constraint on wedding.dress_code_configs.wedding_id.
    /// </summary>
    public WeddingId WeddingId { get; private set; }

    /// <summary>
    /// Narrative description of the expected attire (e.g. "Garden Formal — think soft
    /// florals, linen suits, and earthy tones.").
    /// </summary>
    public string Description { get; private set; }

    public IReadOnlyCollection<DressCodeColor> Colors => _colors.AsReadOnly();

    public IReadOnlyCollection<DressCodeImage> Images => _images.AsReadOnly();

    // ─── Factory ────────────────────────────────────────────────────────────────

    public static DressCodeConfig Create(
        WeddingId weddingId,
        string description)
    {
        var config = new DressCodeConfig
        {
            Id = DressCodeConfigId.New(),
            WeddingId = weddingId,
            Description = description
        };

        config.Raise(new DressCodeConfigCreatedDomainEvent(config.Id, weddingId));

        return config;
    }

    // ─── Description ────────────────────────────────────────────────────────────

    public void UpdateDescription(string description)
    {
        Description = description;
    }

    // ─── Palette colors ─────────────────────────────────────────────────────────

    /// <summary>
    /// Appends a color chip to the palette. The caller is responsible for passing a
    /// fully validated HexColor — use HexColor.Create() in the application layer.
    /// </summary>
    public DressCodeColor AddColor(HexColor colorHex, string colorName, int displayOrder)
    {
        var color = DressCodeColor.Create(Id, colorHex, colorName, displayOrder);
        _colors.Add(color);
        return color;
    }

    public Result UpdateColor(
        DressCodeColorId colorId,
        HexColor colorHex,
        string colorName)
    {
        DressCodeColor? color = _colors.FirstOrDefault(c => c.Id == colorId);

        if (color is null)
        {
            return Result.Failure(DressCodeConfigErrors.ColorNotFound(colorId));
        }

        color.Update(colorHex, colorName);
        return Result.Success();
    }

    public Result ReorderColor(DressCodeColorId colorId, int displayOrder)
    {
        DressCodeColor? color = _colors.FirstOrDefault(c => c.Id == colorId);

        if (color is null)
        {
            return Result.Failure(DressCodeConfigErrors.ColorNotFound(colorId));
        }

        color.Reorder(displayOrder);
        return Result.Success();
    }

    public Result RemoveColor(DressCodeColorId colorId)
    {
        DressCodeColor? color = _colors.FirstOrDefault(c => c.Id == colorId);

        if (color is null)
        {
            return Result.Failure(DressCodeConfigErrors.ColorNotFound(colorId));
        }

        _colors.Remove(color);
        return Result.Success();
    }

    // ─── Inspiration images ──────────────────────────────────────────────────────

    public DressCodeImage AddImage(string imageUrl, int displayOrder)
    {
        var image = DressCodeImage.Create(Id, imageUrl, displayOrder);
        _images.Add(image);
        return image;
    }

    public Result ReorderImage(DressCodeImageId imageId, int displayOrder)
    {
        DressCodeImage? image = _images.FirstOrDefault(i => i.Id == imageId);

        if (image is null)
        {
            return Result.Failure(DressCodeConfigErrors.ImageNotFound(imageId));
        }

        image.Reorder(displayOrder);
        return Result.Success();
    }

    public Result RemoveImage(DressCodeImageId imageId)
    {
        DressCodeImage? image = _images.FirstOrDefault(i => i.Id == imageId);

        if (image is null)
        {
            return Result.Failure(DressCodeConfigErrors.ImageNotFound(imageId));
        }

        _images.Remove(image);
        return Result.Success();
    }
}
