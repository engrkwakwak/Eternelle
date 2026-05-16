using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.UpdateEntourageMember;

internal sealed class UpdateEntourageMemberCommandValidator : AbstractValidator<UpdateEntourageMemberCommand>
{
    public UpdateEntourageMemberCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.EntourageGroupId)
            .NotEmpty();

        RuleFor(c => c.EntourageMemberId)
            .NotEmpty();

        RuleFor(c => c.Name)
            .NotEmpty();

        RuleFor(c => c.Role)
            .NotEmpty();
    }
}
