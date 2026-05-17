using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.GalleryImages;

public static class ImageCaptionErrors
{
    public static readonly Error Empty =
        Error.Problem("ImageCaption.Empty", "Caption must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "ImageCaption.TooLong",
            $"Caption must not exceed {ImageCaption.MaxLength} characters");
}
