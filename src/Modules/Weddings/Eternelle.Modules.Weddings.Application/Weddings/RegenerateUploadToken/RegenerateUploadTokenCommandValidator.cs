using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.Weddings.RegenerateUploadToken;

internal sealed class RegenerateUploadTokenCommandValidator : AbstractValidator<RegenerateUploadTokenCommand>
{
    public RegenerateUploadTokenCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();
    }
}
