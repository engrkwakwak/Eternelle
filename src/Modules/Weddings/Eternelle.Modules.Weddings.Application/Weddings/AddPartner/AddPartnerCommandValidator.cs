using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.Weddings.AddPartner;

internal sealed class AddPartnerCommandValidator : AbstractValidator<AddPartnerCommand>
{
    public AddPartnerCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.PartnerNumber)
            .InclusiveBetween(1, 2);

        RuleFor(c => c.FirstName)
            .NotEmpty();

        RuleFor(c => c.LastName)
            .NotEmpty();
    }
}
