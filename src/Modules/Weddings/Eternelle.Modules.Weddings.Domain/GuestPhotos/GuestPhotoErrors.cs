using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.GuestPhotos;

public static class GuestPhotoErrors
{
    public static readonly Error NotFound =
        Error.NotFound(
            "GuestPhotos.NotFound",
            "The guest photo was not found.");
}
