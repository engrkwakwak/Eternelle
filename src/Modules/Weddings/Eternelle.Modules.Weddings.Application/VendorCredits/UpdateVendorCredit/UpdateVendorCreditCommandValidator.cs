using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.VendorCredits;
using Eternelle.Modules.Weddings.Domain.Weddings;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.VendorCredits.UpdateVendorCredit;

internal sealed class UpdateVendorCreditCommandValidator : AbstractValidator<UpdateVendorCreditCommand>
{
    public UpdateVendorCreditCommandValidator()
    {
        RuleFor(c => c.VendorCreditId)
            .NotEmpty();

        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(VendorCredit.MaxNameLength);

        RuleFor(c => c.Role)
            .NotEmpty()
            .MaximumLength(VendorCredit.MaxRoleLength);

        RuleFor(c => c.InstagramHandle)
            .Custom((handle, ctx) =>
            {
                Result<InstagramHandle> result = InstagramHandle.Create(handle);
                if (result.IsFailure)
                {
                    ctx.AddFailure(result.Error.Description);
                }
            })
            .When(c => !string.IsNullOrWhiteSpace(c.InstagramHandle));
    }
}
