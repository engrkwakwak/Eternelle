using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

/// <summary>
/// Internal-only note visible to the couple (not rendered to guests).
/// Used for reminders like "Needs dietary accommodation" on entourage members
/// or context notes on couple pairings. Up to 500 characters.
/// </summary>
public sealed record InternalNote
{
    public const int MaxLength = 500;

    private InternalNote(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<InternalNote> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<InternalNote>(InternalNoteErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<InternalNote>(InternalNoteErrors.TooLong);
        }

        return Result.Success(new InternalNote(trimmed));
    }

    public override string ToString() => Value;

    internal static InternalNote FromPersistence(string value) => new(value);
}
