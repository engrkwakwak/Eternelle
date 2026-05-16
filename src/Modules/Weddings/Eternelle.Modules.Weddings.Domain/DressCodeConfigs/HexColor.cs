using System.Text.RegularExpressions;
using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.DressCodeConfigs;

/// <summary>
/// Value object representing a CSS hex color code (e.g. #FFF, #AABBCC, #AABBCCFF).
///
/// Stored as entered â€” the domain does not normalize casing.
/// Mirrors the DB-level CHECK constraint: ^#[0-9A-Fa-f]{3,8}$
///
/// Valid lengths after the '#':
///   3  â€” shorthand RGB  (#FFF)
///   4  â€” shorthand RGBA (#FFFA)
///   6  â€” full RGB       (#AABBCC)
///   8  â€” full RGBA      (#AABBCCFF)
/// </summary>
public sealed record HexColor
{
    /// <summary>
    /// Maximum stored length including the leading '#'. Matches the regex upper bound of 8
    /// hex digits plus the '#' prefix.
    /// </summary>
    public const int MaxLength = 9;

    private static readonly Regex Pattern =
        new(@"^#[0-9A-Fa-f]{3,8}$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));

    private HexColor(string value)
    {
        Value = value;
    }

    /// <summary>
    /// The hex color string including the leading '#' (e.g. "#AABBCC").
    /// </summary>
    public string Value { get; }

    public static Result<HexColor> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<HexColor>(HexColorErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (!Pattern.IsMatch(trimmed))
        {
            return Result.Failure<HexColor>(HexColorErrors.InvalidFormat);
        }

        return Result.Success(new HexColor(trimmed));
    }

    public override string ToString() => Value;
}
