using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.GiftOptions.ReorderGiftOptions;

internal sealed class ReorderGiftOptionsCommandValidator : AbstractValidator<ReorderGiftOptionsCommand>
{
    public ReorderGiftOptionsCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.GiftOptionIds)
            .NotEmpty();

        RuleForEach(c => c.GiftOptionIds)
            .NotEmpty();
    }
}
