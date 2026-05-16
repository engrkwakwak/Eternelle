using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.RemoveEntourageCouple;

internal sealed class RemoveEntourageCoupleCommandValidator : AbstractValidator<RemoveEntourageCoupleCommand>
{
    public RemoveEntourageCoupleCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.EntourageGroupId)
            .NotEmpty();

        RuleFor(c => c.EntourageCoupleId)
            .NotEmpty();
    }
}
