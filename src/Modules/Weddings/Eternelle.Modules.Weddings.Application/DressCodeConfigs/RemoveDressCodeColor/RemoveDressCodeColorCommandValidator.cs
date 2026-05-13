using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.RemoveDressCodeColor;

internal sealed class RemoveDressCodeColorCommandValidator : AbstractValidator<RemoveDressCodeColorCommand>
{
    public RemoveDressCodeColorCommandValidator()
    {
        RuleFor(c => c.DressCodeConfigId)
            .NotEmpty();

        RuleFor(c => c.DressCodeColorId)
            .NotEmpty();
    }
}
