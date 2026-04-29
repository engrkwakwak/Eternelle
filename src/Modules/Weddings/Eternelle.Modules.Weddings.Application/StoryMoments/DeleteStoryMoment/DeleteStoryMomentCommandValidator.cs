using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.StoryMoments.DeleteStoryMoment;

internal sealed class DeleteStoryMomentCommandValidator : AbstractValidator<DeleteStoryMomentCommand>
{
    public DeleteStoryMomentCommandValidator()
    {
        RuleFor(c => c.StoryMomentId)
            .NotEmpty();
    }
}
