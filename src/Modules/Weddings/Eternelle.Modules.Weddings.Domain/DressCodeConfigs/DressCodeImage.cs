using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.DressCodeConfigs;

/// <summary>
/// An inspiration image owned by DressCodeConfig (wedding.dress_code_images).
///
/// DressCodeImage is an owned entity — it has no independent lifecycle and is
/// always created and deleted through the DressCodeConfig aggregate root.
/// </summary>
public sealed class DressCodeImage : Entity
{
    private DressCodeImage()
    {
    }

    public DressCodeImageId Id { get; private set; }

    public DressCodeConfigId DressCodeConfigId { get; private set; }

    /// <summary>
    /// CDN or storage URL of the inspiration image. The domain stores, not validates, URLs.
    /// </summary>
    public string ImageUrl { get; private set; }

    public int DisplayOrder { get; private set; }

    // ─── Factory ────────────────────────────────────────────────────────────────

    internal static DressCodeImage Create(
        DressCodeConfigId dressCodeConfigId,
        string imageUrl,
        int displayOrder)
    {
        return new DressCodeImage
        {
            Id = DressCodeImageId.New(),
            DressCodeConfigId = dressCodeConfigId,
            ImageUrl = imageUrl,
            DisplayOrder = displayOrder
        };
    }

    // ─── Behaviour ──────────────────────────────────────────────────────────────

    internal void Reorder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }
}
