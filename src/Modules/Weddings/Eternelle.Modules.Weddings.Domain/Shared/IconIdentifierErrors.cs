using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Shared;

public static class IconIdentifierErrors
{
    public static readonly Error Empty =
        Error.Problem("IconIdentifier.Empty", "Icon must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "IconIdentifier.TooLong",
            $"Icon must not exceed {IconIdentifier.MaxLength} characters");
}
