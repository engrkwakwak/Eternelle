using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.CeremonyActs.DeleteCeremonyAct;

internal sealed class DeleteCeremonyActCommandValidator : AbstractValidator<DeleteCeremonyActCommand>
{
    public DeleteCeremonyActCommandValidator()
    {
        RuleFor(c => c.CeremonyActId)
            .NotEmpty();
    }
}
