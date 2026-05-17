using Eternelle.Modules.Weddings.Domain.EntourageGroups;
using Eternelle.Modules.Weddings.Domain.Shared;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.AddEntourageMember;

internal sealed class AddEntourageMemberCommandValidator : AbstractValidator<AddEntourageMemberCommand>
{
    public AddEntourageMemberCommandValidator()
    {
        RuleFor(c => c.EntourageGroupId)
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

        RuleFor(c => c.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}
