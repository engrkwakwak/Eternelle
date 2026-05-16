using Eternelle.Modules.Weddings.Domain.EntourageGroups;
using Eternelle.Modules.Weddings.Domain.Shared;
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
            .NotEmpty()
            .MaximumLength(PersonName.MaxLength);

        RuleFor(c => c.Role)
            .NotEmpty()
            .MaximumLength(PersonRole.MaxLength);

        RuleFor(c => c.Message)
            .MaximumLength(PersonMessage.MaxLength)
            .When(c => c.Message is not null);

        RuleFor(c => c.Note)
            .MaximumLength(InternalNote.MaxLength)
            .When(c => c.Note is not null);
    }
}
