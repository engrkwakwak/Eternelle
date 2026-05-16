using Eternelle.Modules.Weddings.Domain.Shared;
using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.Reminders.UpdateReminder;

internal sealed class UpdateReminderCommandValidator : AbstractValidator<UpdateReminderCommand>
{
    public UpdateReminderCommandValidator()
    {
        RuleFor(c => c.ReminderId)
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
