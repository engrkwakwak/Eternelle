using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.CeremonyActs.ReorderCeremonyActs;

internal sealed class ReorderCeremonyActsCommandValidator : AbstractValidator<ReorderCeremonyActsCommand>
{
    public ReorderCeremonyActsCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.CeremonyActIds)
            .NotEmpty();

        RuleForEach(c => c.CeremonyActIds)
            .NotEmpty();
    }
}
