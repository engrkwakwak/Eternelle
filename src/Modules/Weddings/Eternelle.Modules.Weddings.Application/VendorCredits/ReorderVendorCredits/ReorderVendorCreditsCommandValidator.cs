using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.VendorCredits.ReorderVendorCredits;

internal sealed class ReorderVendorCreditsCommandValidator : AbstractValidator<ReorderVendorCreditsCommand>
{
    public ReorderVendorCreditsCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.VendorCreditIds)
            .NotEmpty();

        RuleForEach(c => c.VendorCreditIds)
            .NotEmpty();
    }
}
