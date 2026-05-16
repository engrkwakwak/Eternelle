namespace Eternelle.Common.Domain.ValueObjects;

/// <summary>
/// Value object for short titles or names of scheduled or listed items
/// (ceremony acts, gift options, reminders, story moments). Up to 200 characters,
/// trimmed, non-empty.
/// </summary>
public sealed record ActivityName
{
    public static readonly int MaxLength = 200;

    private ActivityName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<ActivityName> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<ActivityName>(ActivityNameErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<ActivityName>(ActivityNameErrors.TooLong);
        }

        return Result.Success(new ActivityName(trimmed));
    }

    public override string ToString() => Value;

    public static ActivityName FromPersistence(string value) => new(value);
}
