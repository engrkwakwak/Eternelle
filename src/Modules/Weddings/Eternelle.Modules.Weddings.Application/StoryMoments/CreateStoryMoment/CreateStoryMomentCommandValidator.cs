using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.StoryMoments.CreateStoryMoment;

internal sealed class CreateStoryMomentCommandValidator : AbstractValidator<CreateStoryMomentCommand>
{
    public CreateStoryMomentCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.Title)
            .NotEmpty();

        RuleFor(c => c.Description)
            .NotEmpty();

        RuleFor(c => c.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}
