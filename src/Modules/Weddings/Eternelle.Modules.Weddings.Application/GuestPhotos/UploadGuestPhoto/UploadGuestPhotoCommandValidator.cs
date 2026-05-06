using Eternelle.Modules.Weddings.Domain.GuestPhotos;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.UploadGuestPhoto;

internal sealed class UploadGuestPhotoCommandValidator : AbstractValidator<UploadGuestPhotoCommand>
{
    public UploadGuestPhotoCommandValidator()
    {
        RuleFor(c => c.UploadToken)
            .NotEmpty();

        RuleFor(c => c.SrcUrl)
            .NotEmpty()
            .MaximumLength(2048);

        RuleFor(c => c.ThumbnailUrl)
            .MaximumLength(2048)
            .When(c => c.ThumbnailUrl is not null);

        RuleFor(c => c.UploaderName)
            .MaximumLength(GuestPhoto.MaxUploaderNameLength)
            .When(c => c.UploaderName is not null);

        RuleFor(c => c.WidthPx)
            .GreaterThan(0)
            .When(c => c.WidthPx.HasValue);

        RuleFor(c => c.HeightPx)
            .GreaterThan(0)
            .When(c => c.HeightPx.HasValue);
    }
}
