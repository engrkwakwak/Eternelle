namespace Eternelle.Modules.Weddings.Domain.Weddings;

/// <summary>
/// Canonical limits for each <see cref="WeddingPlan"/> tier.
/// All limit constants live here — no database rows, no config keys.
/// Changing a tier's limit means changing a constant and running a migration if needed.
/// </summary>
public static class WeddingPlanLimits
{
    public const int FreePhotoLimit = 50;
    public const int ProPhotoLimit = 250;
    // Plus has no photo limit.

    /// <summary>
    /// Maximum number of guest photos for the given plan,
    /// or <c>null</c> if the plan has no limit (unlimited).
    /// </summary>
    public static int? PhotoLimit(WeddingPlan plan) => plan switch
    {
        WeddingPlan.Free => FreePhotoLimit,
        WeddingPlan.Pro  => ProPhotoLimit,
        WeddingPlan.Plus => null,
        _                => FreePhotoLimit
    };

    /// <summary>
    /// Whether the plan allows bulk ZIP download of all guest photos.
    /// </summary>
    public static bool BulkDownloadEnabled(WeddingPlan plan) => plan == WeddingPlan.Plus;
}
