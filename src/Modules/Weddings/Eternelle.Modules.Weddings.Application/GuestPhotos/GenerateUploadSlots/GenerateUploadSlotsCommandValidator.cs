using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.GenerateUploadSlots;

internal sealed class GenerateUploadSlotsCommandValidator : AbstractValidator<GenerateUploadSlotsCommand>
{
    public GenerateUploadSlotsCommandValidator()
    {
        RuleFor(c => c.UploadToken)
            .NotEmpty();

        RuleFor(c => c.Count)
            .GreaterThan(0)
            .LessThanOrEqualTo(GenerateUploadSlotsCommand.MaxBatchSize);
    }
}
