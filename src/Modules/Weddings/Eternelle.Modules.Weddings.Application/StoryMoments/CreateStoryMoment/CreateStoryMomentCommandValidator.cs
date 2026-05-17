using Eternelle.Modules.Weddings.Domain.Shared;
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
            .MaximumLength(ActivityName.MaxLength);

        RuleFor(c => c.Description)
            .NotEmpty()
            .MaximumLength(RichDescription.MaxLength);

        RuleFor(c => c.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}
