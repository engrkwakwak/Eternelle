using Eternelle.Modules.Weddings.Domain.StoryMoments;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.StoryMoments.UpdateStoryMoment;

internal sealed class UpdateStoryMomentCommandValidator : AbstractValidator<UpdateStoryMomentCommand>
{
    public UpdateStoryMomentCommandValidator()
    {
        RuleFor(c => c.StoryMomentId)
            .NotEmpty();

        RuleFor(c => c.Title)
            .NotEmpty()
            .MaximumLength(StoryMoment.MaxTitleLength);

        RuleFor(c => c.Description)
            .NotEmpty()
            .MaximumLength(StoryMoment.MaxDescriptionLength);
    }
}
