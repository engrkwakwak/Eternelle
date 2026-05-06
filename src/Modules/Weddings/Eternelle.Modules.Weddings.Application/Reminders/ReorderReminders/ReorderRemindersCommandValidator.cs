using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.Reminders.ReorderReminders;

internal sealed class ReorderRemindersCommandValidator : AbstractValidator<ReorderRemindersCommand>
{
    public ReorderRemindersCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.ReminderIds)
            .NotEmpty();

        RuleForEach(c => c.ReminderIds)
            .NotEmpty();
    }
}
