using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Shared;

/// <summary>
/// Value object for the display name of a person referenced by a wedding profile
/// (entourage member, guest uploader). Single-field name â€” for partners that
/// split first/last, see <see cref="PersonFirstName"/> and <see cref="PersonLastName"/>.
/// </summary>
public sealed record PersonName
{
    public const int MaxLength = 150;

    private PersonName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<PersonName> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<PersonName>(PersonNameErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<PersonName>(PersonNameErrors.TooLong);
        }

        return Result.Success(new PersonName(trimmed));
    }

    public override string ToString() => Value;

    internal static PersonName FromPersistence(string value) => new(value);
}
