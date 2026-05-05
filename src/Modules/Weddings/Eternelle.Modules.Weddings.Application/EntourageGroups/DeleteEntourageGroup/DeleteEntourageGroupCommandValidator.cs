using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.DeleteEntourageGroup;

internal sealed class DeleteEntourageGroupCommandValidator : AbstractValidator<DeleteEntourageGroupCommand>
{
    public DeleteEntourageGroupCommandValidator()
    {
        RuleFor(c => c.EntourageGroupId)
            .NotEmpty();
    }
}
