using Eternelle.Modules.Weddings.Domain.Shared;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.CeremonyActs.CreateCeremonyAct;

internal sealed class CreateCeremonyActCommandValidator : AbstractValidator<CreateCeremonyActCommand>
{
    public CreateCeremonyActCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(ActivityName.MaxLength);

        RuleFor(c => c.Description)
            .MaximumLength(RichDescription.MaxLength)
            .When(c => c.Description is not null);

        RuleFor(c => c.Icon)
            .MaximumLength(IconIdentifier.MaxLength)
            .When(c => c.Icon is not null);
    }
}
