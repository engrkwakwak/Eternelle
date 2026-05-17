using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.GalleryImages;

/// <summary>
/// Optional caption shown beneath a gallery image. Up to 500 characters.
/// </summary>
public sealed record ImageCaption
{
    public const int MaxLength = 500;

    private ImageCaption(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<ImageCaption> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Result.Failure<ImageCaption>(ImageCaptionErrors.Empty);
        }

        string trimmed = raw.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<ImageCaption>(ImageCaptionErrors.TooLong);
        }

        return Result.Success(new ImageCaption(trimmed));
    }

    public override string ToString() => Value;

    internal static ImageCaption FromPersistence(string value) => new(value);
}
