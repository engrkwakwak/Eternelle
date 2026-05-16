namespace Eternelle.Common.Domain.ValueObjects;

/// <summary>
/// Value object for multi-line descriptive prose (dress code description, story moment body,
/// reminder body, etc.). Allows up to 2000 characters and rejects empty strings — callers
/// that allow an absent description should hold a nullable <see cref="RichDescription"/>?.
/// </summary>
public sealed record RichDescription
{
    public static readonly int MaxLength = 2000;

    private RichDescription(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<RichDescription> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<RichDescription>(RichDescriptionErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<RichDescription>(RichDescriptionErrors.TooLong);
        }

        return Result.Success(new RichDescription(trimmed));
    }

    public override string ToString() => Value;

    public static RichDescription FromPersistence(string value) => new(value);
}
