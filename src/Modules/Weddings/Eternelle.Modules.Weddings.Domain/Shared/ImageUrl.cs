using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Shared;

public sealed record ImageUrl
{
    public static readonly int MaxLength = 2048;

    private ImageUrl(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<ImageUrl> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<ImageUrl>(ImageUrlErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<ImageUrl>(ImageUrlErrors.TooLong);
        }

        if (!Uri.TryCreate(trimmed, UriKind.RelativeOrAbsolute, out _))
        {
            return Result.Failure<ImageUrl>(ImageUrlErrors.InvalidFormat);
        }

        return Result.Success(new ImageUrl(trimmed));
    }

    public override string ToString() => Value;

    internal static ImageUrl FromPersistence(string value) => new(value);
}
