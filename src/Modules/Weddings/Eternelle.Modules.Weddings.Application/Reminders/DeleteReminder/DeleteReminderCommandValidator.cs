using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.Reminders.DeleteReminder;

internal sealed class DeleteReminderCommandValidator : AbstractValidator<DeleteReminderCommand>
{
    public DeleteReminderCommandValidator()
    {
        RuleFor(c => c.ReminderId)
            .NotEmpty();
    }
}
