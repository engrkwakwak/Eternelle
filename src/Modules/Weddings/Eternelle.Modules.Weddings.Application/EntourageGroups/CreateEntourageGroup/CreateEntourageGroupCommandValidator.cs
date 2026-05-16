using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.CreateEntourageGroup;

internal sealed class CreateEntourageGroupCommandValidator : AbstractValidator<CreateEntourageGroupCommand>
{
    public CreateEntourageGroupCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.Label)
            .NotEmpty();

        RuleFor(c => c.GroupType)
            .InclusiveBetween(1, 11)
            .When(c => c.GroupType.HasValue);

        RuleFor(c => c.RenderAs)
            .InclusiveBetween(1, 2);

        RuleFor(c => c.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}
