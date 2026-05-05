using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

/// <summary>
/// Value object representing an Instagram handle (e.g. carlandvina2026).
///
/// Stored without the leading '@' — use ToDisplayString() when rendering to guests.
/// Normalized on creation: leading '@' and surrounding whitespace are stripped.
///
/// Rules follow Instagram's own constraints:
///   - Must not be empty after normalization
///   - Maximum 30 characters
///   - Only letters, digits, underscores, and periods are allowed
/// </summary>
public sealed record InstagramHandle
{
    public static readonly int MaxLength = 30;

    private InstagramHandle(string value)
    {
        Value = value;
    }

    /// <summary>
    /// The normalized handle without the leading '@'.
    /// </summary>
    public string Value { get; }

    public static Result<InstagramHandle> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<InstagramHandle>(InstagramHandleErrors.Empty);
        }

        string normalized = raw.Trim().TrimStart('@').Trim();

        if (normalized.Length == 0)
        {
            return Result.Failure<InstagramHandle>(InstagramHandleErrors.Empty);
        }

        if (normalized.Length > MaxLength)
        {
            return Result.Failure<InstagramHandle>(InstagramHandleErrors.TooLong);
        }

        if (!normalized.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '.'))
        {
            return Result.Failure<InstagramHandle>(InstagramHandleErrors.InvalidCharacters);
        }

        return Result.Success(new InstagramHandle(normalized));
    }

    /// <summary>
    /// Returns the handle as it should appear to guests: @carlandvina2026
    /// </summary>
    public string ToDisplayString() => $"@{Value}";

    public override string ToString() => Value;

    /// <summary>
    /// Bypasses validation and constructs an InstagramHandle directly from a persisted value.
    /// Only for use by EF Core value converters — the value is assumed already valid
    /// because it passed Create() before it was ever written to the database.
    /// </summary>
    internal static InstagramHandle FromPersistence(string value) => new(value);
}
