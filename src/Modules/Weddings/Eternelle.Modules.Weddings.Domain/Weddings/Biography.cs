using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

/// <summary>
/// A short personal blurb for a partner. Optional at the entity level â€”
/// callers hold <see cref="Biography"/>? â€” but if a value is supplied
/// it must be non-blank and bounded.
/// </summary>
public sealed record Biography
{
    public const int MaxLength = 500;

    private Biography(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Biography> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<Biography>(BiographyErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<Biography>(BiographyErrors.TooLong);
        }

        return Result.Success(new Biography(trimmed));
    }

    public override string ToString() => Value;

    internal static Biography FromPersistence(string value) => new(value);
}
