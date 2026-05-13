using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.AddDressCodeColor;

internal sealed class AddDressCodeColorCommandValidator : AbstractValidator<AddDressCodeColorCommand>
{
    public AddDressCodeColorCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.DressCodeConfigId)
            .NotEmpty();

        RuleFor(c => c.ColorHex)
            .NotEmpty()
            .Custom((hex, ctx) =>
            {
                Result<HexColor> result = HexColor.Create(hex);
                if (result.IsFailure)
                {
                    ctx.AddFailure(result.Error.Description);
                }
            });

        RuleFor(c => c.ColorName)
            .NotEmpty()
            .MaximumLength(DressCodeColor.MaxColorNameLength);
    }
}
