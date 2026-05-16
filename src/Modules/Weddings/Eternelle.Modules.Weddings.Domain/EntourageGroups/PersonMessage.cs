using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

/// <summary>
/// Optional short congratulatory or introductory message attached to an
/// entourage member's profile. Up to 1000 characters.
/// </summary>
public sealed record PersonMessage
{
    public static readonly int MaxLength = 1000;

    private PersonMessage(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<PersonMessage> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<PersonMessage>(PersonMessageErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<PersonMessage>(PersonMessageErrors.TooLong);
        }

        return Result.Success(new PersonMessage(trimmed));
    }

    public override string ToString() => Value;

    internal static PersonMessage FromPersistence(string value) => new(value);
}
