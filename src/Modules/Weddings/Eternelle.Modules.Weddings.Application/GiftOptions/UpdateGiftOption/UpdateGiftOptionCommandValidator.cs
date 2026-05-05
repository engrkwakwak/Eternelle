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
            .NotEmpty()
            .MaximumLength(GiftOption.MaxTitleLength);

        RuleFor(c => c.Description)
            .MaximumLength(GiftOption.MaxDescriptionLength)
            .When(c => c.Description is not null);

        RuleFor(c => c.DisplayMode)
            .IsInEnum();

        RuleFor(c => c.LinkUrl)
            .NotEmpty()
            .When(c => c.DisplayMode == GiftDisplayMode.Link);

        RuleFor(c => c.AccountName)
            .MaximumLength(GiftOption.MaxAccountNameLength)
            .When(c => c.AccountName is not null);

        RuleFor(c => c.AccountNumber)
            .MaximumLength(GiftOption.MaxAccountNumberLength)
            .When(c => c.AccountNumber is not null);

        RuleFor(c => c.AccountType)
            .MaximumLength(GiftOption.MaxAccountTypeLength)
            .When(c => c.AccountType is not null);
    }
}
