using Eternelle.Modules.Weddings.Domain.EntourageGroups;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.PairEntourageCouple;

internal sealed class PairEntourageCoupleCommandValidator : AbstractValidator<PairEntourageCoupleCommand>
{
    public PairEntourageCoupleCommandValidator()
    {
        RuleFor(c => c.EntourageGroupId)
            .NotEmpty();

        RuleFor(c => c.MemberAId)
            .NotEmpty();

        RuleFor(c => c.MemberBId)
            .NotEmpty();

        RuleFor(c => c.MemberBId)
            .NotEqual(c => c.MemberAId)
            .WithMessage("A member cannot be paired with themselves.");

        RuleFor(c => c.Note)
            .MaximumLength(EntourageCouple.MaxNoteLength)
            .When(c => c.Note is not null);
    }
}
