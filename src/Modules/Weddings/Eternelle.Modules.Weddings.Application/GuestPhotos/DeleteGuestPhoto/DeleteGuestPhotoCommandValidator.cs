using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.DeleteGuestPhoto;

internal sealed class DeleteGuestPhotoCommandValidator : AbstractValidator<DeleteGuestPhotoCommand>
{
    public DeleteGuestPhotoCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.GuestPhotoId)
            .NotEmpty();
    }
}
