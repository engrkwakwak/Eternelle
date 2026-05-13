using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.GiftOptions.DeleteGiftOption;

internal sealed class DeleteGiftOptionCommandValidator : AbstractValidator<DeleteGiftOptionCommand>
{
    public DeleteGiftOptionCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.GiftOptionId)
            .NotEmpty();
    }
}
