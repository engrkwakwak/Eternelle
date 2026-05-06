using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.Reminders;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.Reminders.ReorderReminders;

internal sealed class ReorderRemindersCommandHandler(
    IReminderRepository reminderRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ReorderRemindersCommand>
{
    public async Task<Result> Handle(
        ReorderRemindersCommand command,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Reminder> reminders = await reminderRepository.GetByWeddingIdAsync(
            new WeddingId(command.WeddingId),
            cancellationToken);

        var remindersById = reminders.ToDictionary(r => r.Id.Value);

        HashSet<Guid> providedIds = [.. command.ReminderIds];

        if (command.ReminderIds.Count != providedIds.Count ||
            providedIds.Count != reminders.Count ||
            !providedIds.SetEquals(remindersById.Keys))
        {
            return Result.Failure(ReminderErrors.ReorderListMismatch);
        }

        for (int i = 0; i < command.ReminderIds.Count; i++)
        {
            Reminder reminder = remindersById[command.ReminderIds[i]];
            reminder.Reorder(i);
            reminderRepository.Update(reminder);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
