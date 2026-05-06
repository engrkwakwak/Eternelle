using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.VendorCredits.DeleteVendorCredit;

internal sealed class DeleteVendorCreditCommandValidator : AbstractValidator<DeleteVendorCreditCommand>
{
    public DeleteVendorCreditCommandValidator()
    {
        RuleFor(c => c.VendorCreditId)
            .NotEmpty();
    }
}
