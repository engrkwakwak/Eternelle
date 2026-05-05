using Eternelle.Modules.Weddings.Domain.Weddings;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.Weddings.UpdatePartner;

internal sealed class UpdatePartnerCommandValidator : AbstractValidator<UpdatePartnerCommand>
{
    public UpdatePartnerCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.PartnerId)
            .NotEmpty();

        RuleFor(c => c.FirstName)
            .NotEmpty()
            .MaximumLength(Partner.MaxFirstNameLength);

        RuleFor(c => c.LastName)
            .NotEmpty()
            .MaximumLength(Partner.MaxLastNameLength);
    }
}
