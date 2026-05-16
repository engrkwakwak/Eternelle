using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.DressCodeConfigs;

public static class ColorNameErrors
{
    public static readonly Error Empty =
        Error.Problem("ColorName.Empty", "Color name must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "ColorName.TooLong",
            $"Color name must not exceed {ColorName.MaxLength} characters");
}
