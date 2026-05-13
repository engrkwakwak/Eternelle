using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.BulkRejectGuestPhotos;

internal sealed class BulkRejectGuestPhotosCommandValidator : AbstractValidator<BulkRejectGuestPhotosCommand>
{
    public BulkRejectGuestPhotosCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.GuestPhotoIds)
            .NotEmpty();

        RuleForEach(c => c.GuestPhotoIds)
            .NotEmpty();
    }
}
