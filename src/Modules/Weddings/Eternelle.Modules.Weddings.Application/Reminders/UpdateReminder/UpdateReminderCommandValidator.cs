using Eternelle.Modules.Weddings.Domain.Reminders;
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
            .MaximumLength(Reminder.MaxIconLength);

        RuleFor(c => c.Title)
            .NotEmpty()
            .MaximumLength(Reminder.MaxTitleLength);

        RuleFor(c => c.Body)
            .NotEmpty()
            .MaximumLength(Reminder.MaxBodyLength);
    }
}
