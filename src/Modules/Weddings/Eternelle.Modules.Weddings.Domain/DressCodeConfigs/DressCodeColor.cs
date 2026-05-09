using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.DressCodeConfigs;

/// <summary>
/// A palette color entry owned by DressCodeConfig (wedding.dress_code_colors).
///
/// DressCodeColor is an owned entity — it has no independent lifecycle and is
/// always created and deleted through the DressCodeConfig aggregate root.
///
/// color_hex is stored as a HexColor value object, mirroring the DB CHECK constraint
/// ^#[0-9A-Fa-f]{3,8}$. color_name is free text (e.g. "Dusty Rose", "Sage Green").
/// </summary>
public sealed class DressCodeColor : Entity
{
    private DressCodeColor()
    {
    }

    public static readonly int MaxColorNameLength = 100;

    public DressCodeColorId Id { get; private set; }

    public DressCodeConfigId DressCodeConfigId { get; private set; }

    public HexColor ColorHex { get; private set; }

    /// <summary>
    /// Human-readable name for the color (e.g. "Dusty Rose").
    /// Free text — no fixed vocabulary; couples define their own palette names.
    /// </summary>
    public string ColorName { get; private set; }

    public int DisplayOrder { get; private set; }

    // ─── Factory ────────────────────────────────────────────────────────────────

    internal static DressCodeColor Create(
        DressCodeConfigId dressCodeConfigId,
        HexColor colorHex,
        string colorName,
        int displayOrder)
    {
        return new DressCodeColor
        {
            Id = DressCodeColorId.New(),
            DressCodeConfigId = dressCodeConfigId,
            ColorHex = colorHex,
            ColorName = colorName,
            DisplayOrder = displayOrder
        };
    }

    // ─── Behaviour ──────────────────────────────────────────────────────────────

    internal void Update(HexColor colorHex, string colorName)
    {
        ColorHex = colorHex;
        ColorName = colorName;
    }

    internal void Reorder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }
}
