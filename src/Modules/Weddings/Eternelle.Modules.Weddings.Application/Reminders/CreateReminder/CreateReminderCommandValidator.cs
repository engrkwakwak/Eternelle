using Eternelle.Modules.Weddings.Domain.Shared;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.Reminders.CreateReminder;

internal sealed class CreateReminderCommandValidator : AbstractValidator<CreateReminderCommand>
{
    public CreateReminderCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.Icon)
            .NotEmpty()
            .MaximumLength(IconIdentifier.MaxLength);

        RuleFor(c => c.Title)
            .NotEmpty()
            .MaximumLength(ActivityName.MaxLength);

        RuleFor(c => c.Body)
            .NotEmpty()
            .MaximumLength(RichDescription.MaxLength);
    }
}
