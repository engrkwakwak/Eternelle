using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.GalleryImages.ReorderGalleryImages;

internal sealed class ReorderGalleryImagesCommandValidator : AbstractValidator<ReorderGalleryImagesCommand>
{
    public ReorderGalleryImagesCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.GalleryImageIds)
            .NotEmpty();

        RuleForEach(c => c.GalleryImageIds)
            .NotEmpty();
    }
}
