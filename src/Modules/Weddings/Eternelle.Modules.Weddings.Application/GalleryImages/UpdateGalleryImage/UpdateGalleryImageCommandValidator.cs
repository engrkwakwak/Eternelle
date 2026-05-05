using Eternelle.Modules.Weddings.Domain.GalleryImages;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.GalleryImages.UpdateGalleryImage;

internal sealed class UpdateGalleryImageCommandValidator : AbstractValidator<UpdateGalleryImageCommand>
{
    public UpdateGalleryImageCommandValidator()
    {
        RuleFor(c => c.GalleryImageId)
            .NotEmpty();

        RuleFor(c => c.SrcUrl)
            .NotEmpty();

        RuleFor(c => c.AltText)
            .NotEmpty()
            .MaximumLength(GalleryImage.MaxAltTextLength);

        RuleFor(c => c.WidthPx)
            .GreaterThan(0)
            .When(c => c.WidthPx.HasValue);

        RuleFor(c => c.HeightPx)
            .GreaterThan(0)
            .When(c => c.HeightPx.HasValue);

        RuleFor(c => c.Caption)
            .MaximumLength(GalleryImage.MaxCaptionLength)
            .When(c => c.Caption is not null);
    }
}
