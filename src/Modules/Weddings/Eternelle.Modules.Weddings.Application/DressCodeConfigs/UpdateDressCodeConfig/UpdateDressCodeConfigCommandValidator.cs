using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.UpdateDressCodeConfig;

internal sealed class UpdateDressCodeConfigCommandValidator : AbstractValidator<UpdateDressCodeConfigCommand>
{
    public UpdateDressCodeConfigCommandValidator()
    {
        RuleFor(c => c.DressCodeConfigId)
            .NotEmpty();

        RuleFor(c => c.Description)
            .NotEmpty()
            .MaximumLength(DressCodeConfig.MaxDescriptionLength);
    }
}
