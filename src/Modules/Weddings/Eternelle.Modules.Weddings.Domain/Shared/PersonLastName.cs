using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Shared;

/// <summary>
/// Last name of a partner. Paired with <see cref="PersonFirstName"/> for partner records.
/// </summary>
public sealed record PersonLastName
{
    public static readonly int MaxLength = 100;

    private PersonLastName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<PersonLastName> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<PersonLastName>(PersonLastNameErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<PersonLastName>(PersonLastNameErrors.TooLong);
        }

        return Result.Success(new PersonLastName(trimmed));
    }

    public override string ToString() => Value;

    internal static PersonLastName FromPersistence(string value) => new(value);
}
