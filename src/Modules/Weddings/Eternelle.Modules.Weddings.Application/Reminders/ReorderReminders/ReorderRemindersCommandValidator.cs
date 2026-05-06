using FluentValidation;

namespace Eternelle.Modules.Weddings.Application.Reminders.ReorderReminders;

internal sealed class ReorderRemindersCommandValidator : AbstractValidator<ReorderRemindersCommand>
{
    public ReorderRemindersCommandValidator()
    {
        RuleFor(c => c.WeddingId)
            .NotEmpty();

        RuleFor(c => c.ReminderIds)
            .NotEmpty()
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("ReminderIds contains duplicate ids.");

        RuleForEach(c => c.ReminderIds)
            .NotEmpty();
    }
}
