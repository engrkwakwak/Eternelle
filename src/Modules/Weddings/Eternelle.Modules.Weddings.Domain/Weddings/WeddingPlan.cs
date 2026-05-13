namespace Eternelle.Modules.Weddings.Domain.Weddings;

/// <summary>
/// Subscription tier for a wedding profile.
/// Stored as an integer column in the database.
/// </summary>
public enum WeddingPlan
{
    /// <summary>
    /// Free tier — up to 50 guest photos, no bulk download.
    /// </summary>
    Free = 0,

    /// <summary>
    /// Pro tier — up to 250 guest photos, no bulk download.
    /// </summary>
    Pro = 1,

    /// <summary>
    /// Plus tier — unlimited guest photos, bulk download enabled.
    /// </summary>
    Plus = 2
}
