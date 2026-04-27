using Eternelle.Modules.Weddings.Domain.Weddings;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.Weddings.CreateWedding;

internal sealed class CreateWeddingCommandValidator : AbstractValidator<CreateWeddingCommand>
{
    public CreateWeddingCommandValidator()
    {
        RuleFor(c => c.TenantId)
            .NotEmpty();

        RuleFor(c => c.WeddingDate)
            .NotEmpty()
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Wedding date must be in the future.");

        RuleFor(c => c.Hashtag)
            .MaximumLength(Hashtag.MaxLength)
            .When(c => c.Hashtag is not null);
    }
}
