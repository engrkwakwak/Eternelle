using Eternelle.Modules.Weddings.Domain.GiftOptions;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.GiftOptions.CreateGiftOption;

internal sealed class CreateGiftOptionCommandValidator : AbstractValidator<CreateGiftOptionCommand>
{
    public CreateGiftOptionCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.Title)
            .NotEmpty();

        RuleFor(c => c.DisplayMode)
            .IsInEnum();

        RuleFor(c => c.LinkUrl)
            .NotEmpty()
            .When(c => c.DisplayMode == GiftDisplayMode.Link);

        RuleFor(c => c.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}
