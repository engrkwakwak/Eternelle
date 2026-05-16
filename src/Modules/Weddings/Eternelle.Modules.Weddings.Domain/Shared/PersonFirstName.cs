using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Shared;

/// <summary>
/// First name of a partner. Paired with <see cref="PersonLastName"/> for partner records;
/// other contexts that need a single display name should use <see cref="PersonName"/>.
/// </summary>
public sealed record PersonFirstName
{
    public static readonly int MaxLength = 100;

    private PersonFirstName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<PersonFirstName> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<PersonFirstName>(PersonFirstNameErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<PersonFirstName>(PersonFirstNameErrors.TooLong);
        }

        return Result.Success(new PersonFirstName(trimmed));
    }

    public override string ToString() => Value;

    internal static PersonFirstName FromPersistence(string value) => new(value);
}
