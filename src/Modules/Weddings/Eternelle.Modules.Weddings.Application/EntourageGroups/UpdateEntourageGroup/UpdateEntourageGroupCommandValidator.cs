using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.UpdateEntourageGroup;

internal sealed class UpdateEntourageGroupCommandValidator : AbstractValidator<UpdateEntourageGroupCommand>
{
    public UpdateEntourageGroupCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.EntourageGroupId)
            .NotEmpty();

        RuleFor(c => c.Label)
            .NotEmpty();

        RuleFor(c => c.GroupType)
            .InclusiveBetween(1, 11)
            .When(c => c.GroupType.HasValue);

        RuleFor(c => c.RenderAs)
            .InclusiveBetween(1, 2);
    }
}
