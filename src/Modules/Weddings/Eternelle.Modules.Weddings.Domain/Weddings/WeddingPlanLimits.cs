namespace Eternelle.Modules.Weddings.Domain.Weddings;

/// <summary>
/// Canonical limits for each <see cref="WeddingPlan"/> tier.
/// All limit constants live here — no database rows, no config keys.
/// To change a wedding's plan, call <see cref="Wedding.ChangePlan"/> on the aggregate.
/// Plan upgrades are driven by the Subscriptions module via integration events —
/// limits here are never adjusted via database migrations or configuration values.
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
        _ => throw new ArgumentOutOfRangeException(nameof(plan), plan, "Unhandled wedding plan.")
    };

    /// <summary>
    /// Whether the plan allows bulk ZIP download of all guest photos.
    /// </summary>
    public static bool BulkDownloadEnabled(WeddingPlan plan) => plan == WeddingPlan.Plus;
}
