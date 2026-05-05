using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.RemoveEntourageCouple;

internal sealed class RemoveEntourageCoupleCommandValidator : AbstractValidator<RemoveEntourageCoupleCommand>
{
    public RemoveEntourageCoupleCommandValidator()
    {
        RuleFor(c => c.EntourageCoupleId)
            .NotEmpty();
    }
}
