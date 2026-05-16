namespace Eternelle.Common.Domain.ValueObjects;

/// <summary>
/// Value object representing the URL of an image. Accepts absolute URIs
/// (https://cdn.example.com/foo.jpg) and absolute-path relative URIs
/// (/uploads/foo.jpg) to cover both CDN and locally-uploaded assets.
/// </summary>
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

    public static ImageUrl FromPersistence(string value) => new(value);
}
