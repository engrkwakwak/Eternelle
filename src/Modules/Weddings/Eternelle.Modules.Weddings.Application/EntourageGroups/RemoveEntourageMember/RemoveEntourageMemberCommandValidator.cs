using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.RemoveEntourageMember;

internal sealed class RemoveEntourageMemberCommandValidator : AbstractValidator<RemoveEntourageMemberCommand>
{
    public RemoveEntourageMemberCommandValidator()
    {
        RuleFor(c => c.EntourageMemberId)
            .NotEmpty();
    }
}
