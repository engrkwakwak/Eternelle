using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.RejectGuestPhoto;

internal sealed class RejectGuestPhotoCommandValidator : AbstractValidator<RejectGuestPhotoCommand>
{
    public RejectGuestPhotoCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.GuestPhotoId)
            .NotEmpty();
    }
}
