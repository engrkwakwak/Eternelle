using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.GuestPhotos;

public static class GuestPhotoErrors
{
    public static readonly Error NotFound =
        Error.NotFound(
            "GuestPhotos.NotFound",
            "The guest photo was not found.");

    public static readonly Error AlreadyReviewed =
        Error.Conflict(
            "GuestPhotos.AlreadyReviewed",
            "The guest photo has already been reviewed.");

    public static readonly Error PlanLimitReached =
        Error.Problem(
            "GuestPhotos.PlanLimitReached",
            "The photo upload limit for this wedding's plan has been reached.");
}
