using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Shared;

public sealed record ActivityName
{
    public const int MaxLength = 200;

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

    internal static ActivityName FromPersistence(string value) => new(value);
}
