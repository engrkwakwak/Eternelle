using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.ReorderEntourageMembers;

internal sealed class ReorderEntourageMembersCommandValidator : AbstractValidator<ReorderEntourageMembersCommand>
{
    public ReorderEntourageMembersCommandValidator()
    {
        RuleFor(c => c.EntourageGroupId)
            .NotEmpty();

        RuleFor(c => c.EntourageMemberIds)
            .NotEmpty();
    }
}
