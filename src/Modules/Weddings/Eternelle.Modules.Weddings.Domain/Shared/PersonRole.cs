using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Shared;

/// <summary>
/// Free-text role label for a person within a wedding (entourage member, vendor).
/// Examples: "Maid of Honor", "Best Man", "Photographer". Up to 100 characters.
/// </summary>
public sealed record PersonRole
{
    public const int MaxLength = 100;

    private PersonRole(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<PersonRole> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<PersonRole>(PersonRoleErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<PersonRole>(PersonRoleErrors.TooLong);
        }

        return Result.Success(new PersonRole(trimmed));
    }

    public override string ToString() => Value;

    internal static PersonRole FromPersistence(string value) => new(value);
}
