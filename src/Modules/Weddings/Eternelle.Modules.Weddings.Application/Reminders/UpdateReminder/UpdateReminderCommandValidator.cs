using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.Reminders.UpdateReminder;

internal sealed class UpdateReminderCommandValidator : AbstractValidator<UpdateReminderCommand>
{
    public UpdateReminderCommandValidator()
    {
        RuleFor(c => c.ReminderId)
            .NotEmpty();

        RuleFor(c => c.Icon)
            .NotEmpty();

        RuleFor(c => c.Title)
            .NotEmpty();

        RuleFor(c => c.Body)
            .NotEmpty();
    }
}
