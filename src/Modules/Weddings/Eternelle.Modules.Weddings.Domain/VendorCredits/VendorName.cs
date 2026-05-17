using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.VendorCredits;

/// <summary>
/// Name of a vendor (photographer, florist, caterer) credited on the wedding.
/// Up to 200 characters.
/// </summary>
public sealed record VendorName
{
    public const int MaxLength = 200;

    private VendorName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<VendorName> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<VendorName>(VendorNameErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<VendorName>(VendorNameErrors.TooLong);
        }

        return Result.Success(new VendorName(trimmed));
    }

    public override string ToString() => Value;

    internal static VendorName FromPersistence(string value) => new(value);
}
