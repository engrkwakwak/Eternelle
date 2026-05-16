using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Shared;

internal static class ImageUrlErrors
{
    public static readonly Error Empty =
        Error.Problem("ImageUrl.Empty", "Image URL must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "ImageUrl.TooLong",
            $"Image URL must not exceed {ImageUrl.MaxLength} characters");

    public static readonly Error InvalidFormat =
        Error.Problem(
            "ImageUrl.InvalidFormat",
            "Image URL must be a valid URI");
}
