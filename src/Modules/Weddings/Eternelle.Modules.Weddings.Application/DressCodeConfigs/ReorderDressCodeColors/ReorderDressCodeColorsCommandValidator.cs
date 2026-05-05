using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.ReorderDressCodeColors;

internal sealed class ReorderDressCodeColorsCommandValidator : AbstractValidator<ReorderDressCodeColorsCommand>
{
    public ReorderDressCodeColorsCommandValidator()
    {
        RuleFor(c => c.DressCodeConfigId)
            .NotEmpty();

        RuleFor(c => c.DressCodeColorIds)
            .NotEmpty();

        RuleForEach(c => c.DressCodeColorIds)
            .NotEmpty();
    }
}
