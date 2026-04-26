using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.GiftOptions;

public static class GiftOptionErrors
{
    public static Error NotFound(GiftOptionId id) =>
        Error.NotFound(
            "GiftOptions.NotFound",
            $"The gift option with the identifier {id.Value} was not found");

    public static readonly Error LinkUrlRequired =
        Error.Problem(
            "GiftOptions.LinkUrlRequired",
            "A link URL is required when the display mode is 'Link'");
}
