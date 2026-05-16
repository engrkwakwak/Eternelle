using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

/// <summary>
/// Value object representing a wedding hashtag (e.g. CarlAndVina2026).
///
/// Stored without the leading '#' â€” use ToDisplayString() when rendering to guests.
/// Normalized on creation: leading '#' and surrounding whitespace are stripped.
///
/// Rules:
///   - Must not be empty after normalization
///   - Must not contain spaces
///   - Only letters, digits, and underscores are allowed
///   - Maximum 100 characters (wedding hashtags are long by nature)
/// </summary>
public sealed record Hashtag
{
    public const int MaxLength = 100;

    private Hashtag(string value)
    {
        Value = value;
    }

    /// <summary>
    /// The normalized hashtag value without the leading '#'.
    /// </summary>
    public string Value { get; }

    public static Result<Hashtag> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<Hashtag>(HashtagErrors.Empty);
        }

        string normalized = raw.Trim().TrimStart('#').Trim();

        if (normalized.Length == 0)
        {
            return Result.Failure<Hashtag>(HashtagErrors.Empty);
        }

        if (normalized.Length > MaxLength)
        {
            return Result.Failure<Hashtag>(HashtagErrors.TooLong);
        }

        if (normalized.Contains(' '))
        {
            return Result.Failure<Hashtag>(HashtagErrors.ContainsSpaces);
        }

        if (!normalized.All(c => char.IsLetterOrDigit(c) || c == '_'))
        {
            return Result.Failure<Hashtag>(HashtagErrors.InvalidCharacters);
        }

        return Result.Success(new Hashtag(normalized));
    }

    /// <summary>
    /// Returns the hashtag as it should appear to guests: #CarlAndVina2026
    /// </summary>
    public string ToDisplayString() => $"#{Value}";

    // Record auto-generates Equals() and GetHashCode() from Value â€” no manual override needed.
    // ToString() is overridden because the record default would produce "Hashtag { Value = ... }".
    public override string ToString() => Value;

    /// <summary>
    /// Bypasses validation and constructs a Hashtag directly from a persisted value.
    /// Only for use by EF Core value converters â€” the value is assumed already valid
    /// because it passed Create() before it was ever written to the database.
    /// </summary>
    internal static Hashtag FromPersistence(string value) => new(value);
}
