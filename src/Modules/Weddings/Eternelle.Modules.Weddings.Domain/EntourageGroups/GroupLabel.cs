using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

/// <summary>
/// Display label for an entourage group (e.g. "Ninongs &amp; Ninangs",
/// "Secondary Sponsors"). Free text — couples often use Filipino-specific labels.
/// </summary>
public sealed record GroupLabel
{
    public static readonly int MaxLength = 150;

    private GroupLabel(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<GroupLabel> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<GroupLabel>(GroupLabelErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<GroupLabel>(GroupLabelErrors.TooLong);
        }

        return Result.Success(new GroupLabel(trimmed));
    }

    public override string ToString() => Value;

    internal static GroupLabel FromPersistence(string value) => new(value);
}
