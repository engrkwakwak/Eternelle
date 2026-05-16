using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.Reminders.CreateReminder;

internal sealed class CreateReminderCommandValidator : AbstractValidator<CreateReminderCommand>
{
    public CreateReminderCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.Icon)
            .NotEmpty();

        RuleFor(c => c.Title)
            .NotEmpty();

        RuleFor(c => c.Body)
            .NotEmpty();
    }
}
