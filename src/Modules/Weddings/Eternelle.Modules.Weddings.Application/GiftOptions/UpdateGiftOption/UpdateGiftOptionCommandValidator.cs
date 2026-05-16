using Eternelle.Modules.Weddings.Domain.GiftOptions;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.GiftOptions.UpdateGiftOption;

internal sealed class UpdateGiftOptionCommandValidator : AbstractValidator<UpdateGiftOptionCommand>
{
    public UpdateGiftOptionCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.GiftOptionId)
            .NotEmpty();

        RuleFor(c => c.Title)
            .NotEmpty();

        RuleFor(c => c.DisplayMode)
            .IsInEnum();

        RuleFor(c => c.LinkUrl)
            .NotEmpty()
            .When(c => c.DisplayMode == GiftDisplayMode.Link);
    }
}
