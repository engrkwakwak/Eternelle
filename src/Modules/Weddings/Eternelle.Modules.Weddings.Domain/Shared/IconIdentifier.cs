using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Shared;

/// <summary>
/// Short identifier for a visual icon — typically an emoji glyph or a short icon-set key
/// (e.g. "ring", "cake"). Up to 50 characters so multi-codepoint emoji clusters fit.
/// </summary>
public sealed record IconIdentifier
{
    public static readonly int MaxLength = 50;

    private IconIdentifier(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<IconIdentifier> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<IconIdentifier>(IconIdentifierErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<IconIdentifier>(IconIdentifierErrors.TooLong);
        }

        return Result.Success(new IconIdentifier(trimmed));
    }

    public override string ToString() => Value;

    internal static IconIdentifier FromPersistence(string value) => new(value);
}
