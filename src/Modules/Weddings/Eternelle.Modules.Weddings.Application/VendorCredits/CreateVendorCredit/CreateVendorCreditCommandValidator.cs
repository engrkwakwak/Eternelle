using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.VendorCredits;
using Eternelle.Modules.Weddings.Domain.Weddings;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.VendorCredits.CreateVendorCredit;

internal sealed class CreateVendorCreditCommandValidator : AbstractValidator<CreateVendorCreditCommand>
{
    public CreateVendorCreditCommandValidator()
    {
        RuleFor(c => c.WeddingId)
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
