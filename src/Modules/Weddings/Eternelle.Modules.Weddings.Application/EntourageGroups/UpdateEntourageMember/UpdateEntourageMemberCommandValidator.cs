using Eternelle.Modules.Weddings.Domain.EntourageGroups;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.UpdateEntourageMember;

internal sealed class UpdateEntourageMemberCommandValidator : AbstractValidator<UpdateEntourageMemberCommand>
{
    public UpdateEntourageMemberCommandValidator()
    {
        RuleFor(c => c.EntourageMemberId)
            .NotEmpty();

        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(EntourageMember.MaxNameLength);

        RuleFor(c => c.Role)
            .NotEmpty()
            .MaximumLength(EntourageMember.MaxRoleLength);

        RuleFor(c => c.Message)
            .MaximumLength(EntourageMember.MaxMessageLength)
            .When(c => c.Message is not null);

        RuleFor(c => c.Note)
            .MaximumLength(EntourageMember.MaxNoteLength)
            .When(c => c.Note is not null);
    }
}
