using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.ReorderEntourageGroups;

internal sealed class ReorderEntourageGroupsCommandValidator : AbstractValidator<ReorderEntourageGroupsCommand>
{
    public ReorderEntourageGroupsCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.EntourageGroupIds)
            .NotEmpty();

        RuleForEach(c => c.EntourageGroupIds)
            .NotEmpty();
    }
}
