using Eternelle.Modules.Weddings.Domain.Weddings;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.Weddings.UpdateWeddingDetails;

internal sealed class UpdateWeddingDetailsCommandValidator : AbstractValidator<UpdateWeddingDetailsCommand>
{
    public UpdateWeddingDetailsCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.WeddingDate)
            .NotEmpty();

        RuleFor(c => c.Hashtag)
            .MaximumLength(Hashtag.MaxLength)
            .When(c => c.Hashtag is not null);
    }
}
