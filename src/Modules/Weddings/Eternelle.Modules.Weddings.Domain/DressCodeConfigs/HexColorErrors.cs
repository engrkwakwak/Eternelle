using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.DressCodeConfigs;

public static class HexColorErrors
{
    public static readonly Error Empty =
        Error.Problem("HexColor.Empty", "Hex color must not be empty");

    public static readonly Error InvalidFormat =
        Error.Problem(
            "HexColor.InvalidFormat",
            "Hex color must be a valid CSS hex value (e.g. #FFF, #AABBCC, #AABBCCFF)");
}
