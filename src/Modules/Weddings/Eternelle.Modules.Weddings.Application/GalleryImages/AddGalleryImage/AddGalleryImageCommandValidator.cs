using Eternelle.Modules.Weddings.Domain.GalleryImages;
using Eternelle.Modules.Weddings.Domain.Shared;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.GalleryImages.AddGalleryImage;

internal sealed class AddGalleryImageCommandValidator : AbstractValidator<AddGalleryImageCommand>
{
    public AddGalleryImageCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.SrcUrl)
            .NotEmpty()
            .MaximumLength(ImageUrl.MaxLength);

        RuleFor(c => c.AltText)
            .NotEmpty()
            .MaximumLength(AccessibilityText.MaxLength);

        RuleFor(c => c.Caption)
            .MaximumLength(ImageCaption.MaxLength)
            .When(c => c.Caption is not null);

        RuleFor(c => c.WidthPx)
            .GreaterThan(0)
            .When(c => c.WidthPx.HasValue);

        RuleFor(c => c.HeightPx)
            .GreaterThan(0)
            .When(c => c.HeightPx.HasValue);

        RuleFor(c => c.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}
