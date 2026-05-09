using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.RemoveDressCodeImage;

internal sealed class RemoveDressCodeImageCommandValidator : AbstractValidator<RemoveDressCodeImageCommand>
{
    public RemoveDressCodeImageCommandValidator()
    {
        RuleFor(c => c.DressCodeImageId)
            .NotEmpty();
    }
}
