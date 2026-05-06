using Eternelle.Modules.Weddings.Domain.Reminders;
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
            .MaximumLength(Reminder.MaxIconLength);

        RuleFor(c => c.Title)
            .NotEmpty()
            .MaximumLength(Reminder.MaxTitleLength);

        RuleFor(c => c.Body)
            .NotEmpty()
            .MaximumLength(Reminder.MaxBodyLength);
    }
}
