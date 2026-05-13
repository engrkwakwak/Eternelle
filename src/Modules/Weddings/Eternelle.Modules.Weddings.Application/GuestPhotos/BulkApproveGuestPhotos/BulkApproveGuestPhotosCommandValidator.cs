using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.BulkApproveGuestPhotos;

internal sealed class BulkApproveGuestPhotosCommandValidator : AbstractValidator<BulkApproveGuestPhotosCommand>
{
    public BulkApproveGuestPhotosCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.GuestPhotoIds)
            .NotEmpty();

        RuleForEach(c => c.GuestPhotoIds)
            .NotEmpty();
    }
}
