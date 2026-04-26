using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.GalleryImages;

public static class GalleryImageErrors
{
    public static Error NotFound(GalleryImageId id) =>
        Error.NotFound(
            "GalleryImages.NotFound",
            $"The gallery image with the identifier {id.Value} was not found");
}
