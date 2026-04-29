using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.StoryMoments.ReorderStoryMoments;

internal sealed class ReorderStoryMomentsCommandValidator : AbstractValidator<ReorderStoryMomentsCommand>
{
    public ReorderStoryMomentsCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.StoryMomentIds)
            .NotEmpty();

        RuleForEach(c => c.StoryMomentIds)
            .NotEmpty();
    }
}
