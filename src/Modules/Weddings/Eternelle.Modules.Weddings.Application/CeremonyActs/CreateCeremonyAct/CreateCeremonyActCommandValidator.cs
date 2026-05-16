using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.CeremonyActs.CreateCeremonyAct;

internal sealed class CreateCeremonyActCommandValidator : AbstractValidator<CreateCeremonyActCommand>
{
    public CreateCeremonyActCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.Name)
            .NotEmpty();
    }
}
