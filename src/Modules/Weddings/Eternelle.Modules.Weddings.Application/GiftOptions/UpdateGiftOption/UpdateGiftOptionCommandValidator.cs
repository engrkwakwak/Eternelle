using Eternelle.Modules.Weddings.Domain.GiftOptions;
using Eternelle.Modules.Weddings.Domain.Shared;
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
            .MaximumLength(ActivityName.MaxLength);

        RuleFor(c => c.Description)
            .MaximumLength(RichDescription.MaxLength)
            .When(c => c.Description is not null);

        RuleFor(c => c.DisplayMode)
            .IsInEnum();

        RuleFor(c => c.LinkUrl)
            .NotEmpty()
            .When(c => c.DisplayMode == GiftDisplayMode.Link);

        RuleFor(c => c.LinkUrl)
            .MaximumLength(WebUrl.MaxLength)
            .When(c => c.LinkUrl is not null);

        RuleFor(c => c.ImageUrl)
            .MaximumLength(ImageUrl.MaxLength)
            .When(c => c.ImageUrl is not null);

        RuleFor(c => c.QrImageUrl)
            .MaximumLength(ImageUrl.MaxLength)
            .When(c => c.QrImageUrl is not null);

        RuleFor(c => c.AccountName)
            .MaximumLength(AccountHolderName.MaxLength)
            .When(c => c.AccountName is not null);

        RuleFor(c => c.AccountNumber)
            .MaximumLength(AccountNumber.MaxLength)
            .When(c => c.AccountNumber is not null);

        RuleFor(c => c.AccountType)
            .MaximumLength(AccountType.MaxLength)
            .When(c => c.AccountType is not null);
    }
}
