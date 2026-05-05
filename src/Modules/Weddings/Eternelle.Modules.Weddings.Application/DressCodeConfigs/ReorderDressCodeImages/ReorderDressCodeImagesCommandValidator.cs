using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.ReorderDressCodeImages;

internal sealed class ReorderDressCodeImagesCommandValidator : AbstractValidator<ReorderDressCodeImagesCommand>
{
    public ReorderDressCodeImagesCommandValidator()
    {
        RuleFor(c => c.DressCodeConfigId)
            .NotEmpty();

        RuleFor(c => c.DressCodeImageIds)
            .NotEmpty();

        RuleForEach(c => c.DressCodeImageIds)
            .NotEmpty();
    }
}
