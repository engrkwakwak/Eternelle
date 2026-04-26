using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.DressCodeConfigs;

public static class DressCodeConfigErrors
{
    public static Error NotFound(DressCodeConfigId id) =>
        Error.NotFound(
            "DressCodeConfigs.NotFound",
            $"The dress code config with the identifier {id.Value} was not found");

    public static Error NotFoundForWedding(Guid weddingId) =>
        Error.NotFound(
            "DressCodeConfigs.NotFoundForWedding",
            $"No dress code config exists for the wedding with identifier {weddingId}");

    public static Error ColorNotFound(DressCodeColorId colorId) =>
        Error.NotFound(
            "DressCodeConfigs.ColorNotFound",
            $"The palette color with the identifier {colorId.Value} was not found");

    public static Error ImageNotFound(DressCodeImageId imageId) =>
        Error.NotFound(
            "DressCodeConfigs.ImageNotFound",
            $"The dress code image with the identifier {imageId.Value} was not found");
}
