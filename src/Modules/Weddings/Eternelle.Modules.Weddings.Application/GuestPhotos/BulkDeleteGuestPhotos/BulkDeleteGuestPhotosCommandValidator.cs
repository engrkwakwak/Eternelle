using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.BulkDeleteGuestPhotos;

internal sealed class BulkDeleteGuestPhotosCommandValidator : AbstractValidator<BulkDeleteGuestPhotosCommand>
{
    public BulkDeleteGuestPhotosCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.GuestPhotoIds)
            .NotEmpty();

        RuleForEach(c => c.GuestPhotoIds)
            .NotEmpty();
    }
}
