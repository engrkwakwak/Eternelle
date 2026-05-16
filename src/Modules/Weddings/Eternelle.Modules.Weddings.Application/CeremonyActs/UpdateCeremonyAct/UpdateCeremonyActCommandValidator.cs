using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.CeremonyActs.UpdateCeremonyAct;

internal sealed class UpdateCeremonyActCommandValidator : AbstractValidator<UpdateCeremonyActCommand>
{
    public UpdateCeremonyActCommandValidator()
    {
        RuleFor(c => c.CeremonyActId)
            .NotEmpty();

        RuleFor(c => c.Name)
            .NotEmpty();
    }
}
