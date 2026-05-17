using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Shared;

public sealed record RichDescription
{
    public const int MaxLength = 2000;

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

    internal static RichDescription FromPersistence(string value) => new(value);
}
