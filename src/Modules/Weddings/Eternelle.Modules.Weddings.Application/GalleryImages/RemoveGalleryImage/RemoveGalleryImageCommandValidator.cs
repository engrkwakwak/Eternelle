using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.GalleryImages.RemoveGalleryImage;

internal sealed class RemoveGalleryImageCommandValidator : AbstractValidator<RemoveGalleryImageCommand>
{
    public RemoveGalleryImageCommandValidator()
    {
        RuleFor(c => c.GalleryImageId)
            .NotEmpty();
    }
}
