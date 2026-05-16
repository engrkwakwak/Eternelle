using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

/// <summary>
/// Optional secondary label for an entourage group, shown under the main label
/// in some render modes. Up to 200 characters.
/// </summary>
public sealed record GroupSubtitle
{
    public const int MaxLength = 200;

    private GroupSubtitle(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<GroupSubtitle> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<GroupSubtitle>(GroupSubtitleErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<GroupSubtitle>(GroupSubtitleErrors.TooLong);
        }

        return Result.Success(new GroupSubtitle(trimmed));
    }

    public override string ToString() => Value;

    internal static GroupSubtitle FromPersistence(string value) => new(value);
}
