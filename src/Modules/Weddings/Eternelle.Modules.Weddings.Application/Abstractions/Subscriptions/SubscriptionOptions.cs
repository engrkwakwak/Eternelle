namespace Eternelle.Modules.Weddings.Application.Abstractions.Subscriptions;

/// <summary>
/// Configuration-backed subscription limits used by the stub implementation
/// until the real Subscriptions module is available.
/// </summary>
public sealed class SubscriptionOptions
{
    public const string SectionName = "Weddings:Subscription";

    /// <summary>
    /// Maximum number of guest photos per wedding.
    /// <c>null</c> means unlimited.
    /// </summary>
    public int? PhotoUploadLimit { get; init; }
}
