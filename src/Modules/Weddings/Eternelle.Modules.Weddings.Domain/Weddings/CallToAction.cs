using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

/// <summary>
/// Optional call-to-action shown to guests on the snap-share section
/// (e.g. "Tag us @carlandvina2026 in your photos!"). Up to 200 characters.
/// </summary>
public sealed record CallToAction
{
    public const int MaxLength = 200;

    private CallToAction(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<CallToAction> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<CallToAction>(CallToActionErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<CallToAction>(CallToActionErrors.TooLong);
        }

        return Result.Success(new CallToAction(trimmed));
    }

    public override string ToString() => Value;

    internal static CallToAction FromPersistence(string value) => new(value);
}
