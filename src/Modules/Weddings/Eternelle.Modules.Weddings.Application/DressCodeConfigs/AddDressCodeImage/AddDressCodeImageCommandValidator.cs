using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.AddDressCodeImage;

internal sealed class AddDressCodeImageCommandValidator : AbstractValidator<AddDressCodeImageCommand>
{
    public AddDressCodeImageCommandValidator()
    {
        RuleFor(c => c.DressCodeConfigId)
            .NotEmpty();

        RuleFor(c => c.ImageUrl)
            .NotEmpty();
    }
}
