using Eternelle.Modules.Weddings.Domain.CeremonyActs;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.CeremonyActs.UpdateCeremonyAct;

internal sealed class UpdateCeremonyActCommandValidator : AbstractValidator<UpdateCeremonyActCommand>
{
    public UpdateCeremonyActCommandValidator()
    {
        RuleFor(c => c.CeremonyActId)
            .NotEmpty();

        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(CeremonyAct.MaxNameLength);

        RuleFor(c => c.Description)
            .MaximumLength(CeremonyAct.MaxDescriptionLength)
            .When(c => c.Description is not null);

        RuleFor(c => c.Icon)
            .MaximumLength(CeremonyAct.MaxIconLength)
            .When(c => c.Icon is not null);
    }
}
