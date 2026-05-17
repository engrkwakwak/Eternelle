using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.GalleryImages;

public static class AccessibilityTextErrors
{
    public static readonly Error Empty =
        Error.Problem("AccessibilityText.Empty", "Alt text must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "AccessibilityText.TooLong",
            $"Alt text must not exceed {AccessibilityText.MaxLength} characters");
}
