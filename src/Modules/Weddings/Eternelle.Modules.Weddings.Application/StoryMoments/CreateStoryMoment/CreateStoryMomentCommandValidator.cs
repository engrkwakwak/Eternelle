using Eternelle.Modules.Weddings.Domain.StoryMoments;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.StoryMoments.CreateStoryMoment;

internal sealed class CreateStoryMomentCommandValidator : AbstractValidator<CreateStoryMomentCommand>
{
    public CreateStoryMomentCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.Title)
            .NotEmpty()
            .MaximumLength(StoryMoment.MaxTitleLength);

        RuleFor(c => c.Description)
            .NotEmpty()
            .MaximumLength(StoryMoment.MaxDescriptionLength);

        RuleFor(c => c.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}
