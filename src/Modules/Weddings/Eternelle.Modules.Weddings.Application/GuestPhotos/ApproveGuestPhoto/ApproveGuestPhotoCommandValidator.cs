using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.ApproveGuestPhoto;

internal sealed class ApproveGuestPhotoCommandValidator : AbstractValidator<ApproveGuestPhotoCommand>
{
    public ApproveGuestPhotoCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.GuestPhotoId)
            .NotEmpty();
    }
}
