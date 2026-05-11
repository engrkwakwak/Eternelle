using Eternelle.Modules.Weddings.Domain.Weddings;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.Weddings.UpdateSnapShareConfig;

internal sealed class UpdateSnapShareConfigCommandValidator : AbstractValidator<UpdateSnapShareConfigCommand>
{
    public UpdateSnapShareConfigCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.InstagramHandle)
            .MaximumLength(InstagramHandle.MaxLength)
            .When(c => c.InstagramHandle is not null);

        RuleFor(c => c.CtaText)
            .MaximumLength(SnapShareConfig.MaxCtaTextLength)
            .When(c => c.CtaText is not null);

        RuleFor(c => c.ModerationMode)
            .IsInEnum();
    }
}
