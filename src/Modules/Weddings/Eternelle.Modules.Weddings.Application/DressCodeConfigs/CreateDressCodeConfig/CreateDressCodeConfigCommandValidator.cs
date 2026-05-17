using Eternelle.Modules.Weddings.Domain.Shared;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.CreateDressCodeConfig;

internal sealed class CreateDressCodeConfigCommandValidator : AbstractValidator<CreateDressCodeConfigCommand>
{
    public CreateDressCodeConfigCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.Description)
            .NotEmpty()
            .MaximumLength(RichDescription.MaxLength);
    }
}
