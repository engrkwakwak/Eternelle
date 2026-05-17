using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.GalleryImages;

/// <summary>
/// Alt text for a gallery image â€” required for accessibility.
/// Up to 300 characters; describes the image content for screen readers.
/// </summary>
public sealed record AccessibilityText
{
    public const int MaxLength = 300;

    private AccessibilityText(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<AccessibilityText> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<AccessibilityText>(AccessibilityTextErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<AccessibilityText>(AccessibilityTextErrors.TooLong);
        }

        return Result.Success(new AccessibilityText(trimmed));
    }

    public override string ToString() => Value;

    internal static AccessibilityText FromPersistence(string value) => new(value);
}
