using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.AddEntourageMember;

internal sealed class AddEntourageMemberCommandValidator : AbstractValidator<AddEntourageMemberCommand>
{
    public AddEntourageMemberCommandValidator()
    {
        RuleFor(c => c.EntourageGroupId)
            .NotEmpty();

        RuleFor(c => c.Name)
            .NotEmpty();

        RuleFor(c => c.Role)
            .NotEmpty();

        RuleFor(c => c.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}
