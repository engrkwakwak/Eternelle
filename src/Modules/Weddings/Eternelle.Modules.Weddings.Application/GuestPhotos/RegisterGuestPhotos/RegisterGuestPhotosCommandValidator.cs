using Eternelle.Modules.Weddings.Application.GuestPhotos.GenerateUploadSlots;
using Eternelle.Modules.Weddings.Domain.Shared;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.RegisterGuestPhotos;

internal sealed class RegisterGuestPhotosCommandValidator : AbstractValidator<RegisterGuestPhotosCommand>
{
    public RegisterGuestPhotosCommandValidator()
    {
        RuleFor(c => c.UploadToken)
            .NotEmpty();

        RuleFor(c => c.Photos)
            .NotEmpty()
            .Must(p => p.Count <= GenerateUploadSlotsCommand.MaxBatchSize)
            .WithMessage($"Cannot register more than {GenerateUploadSlotsCommand.MaxBatchSize} photos at once.");

        RuleForEach(c => c.Photos).ChildRules(photo =>
        {
            photo.RuleFor(p => p.SlotId)
                .NotEmpty();

            photo.RuleFor(p => p.UploaderName)
                .NotEmpty()
                .MaximumLength(PersonName.MaxLength)
                .When(p => p.UploaderName is not null);

            photo.RuleFor(p => p.WidthPx)
                .GreaterThan(0)
                .When(p => p.WidthPx.HasValue);

            photo.RuleFor(p => p.HeightPx)
                .GreaterThan(0)
                .When(p => p.HeightPx.HasValue);
        });
    }
}
