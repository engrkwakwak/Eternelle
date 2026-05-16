using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.DressCodeConfigs;

/// <summary>
/// Human-readable name of a palette color (e.g. "Dusty Rose", "Sage Green").
/// Free text — couples define their own palette names. Up to 100 characters.
/// </summary>
public sealed record ColorName
{
    public static readonly int MaxLength = 100;

    private ColorName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<ColorName> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<ColorName>(ColorNameErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<ColorName>(ColorNameErrors.TooLong);
        }

        return Result.Success(new ColorName(trimmed));
    }

    public override string ToString() => Value;

    internal static ColorName FromPersistence(string value) => new(value);
}
