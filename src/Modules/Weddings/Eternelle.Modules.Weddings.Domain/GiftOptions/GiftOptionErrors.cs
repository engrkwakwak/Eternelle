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

    public static readonly Error ReorderListMismatch =
        Error.Problem(
            "GiftOptions.ReorderListMismatch",
            "The provided gift option IDs do not match the existing gift options for this wedding");
}
