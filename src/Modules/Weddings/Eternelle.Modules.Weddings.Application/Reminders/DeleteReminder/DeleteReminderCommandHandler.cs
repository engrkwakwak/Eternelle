using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.Reminders;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.Reminders.DeleteReminder;

internal sealed class DeleteReminderCommandHandler(
    IReminderRepository reminderRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteReminderCommand>
{
    public async Task<Result> Handle(
        DeleteReminderCommand command,
        CancellationToken cancellationToken)
    {
        var reminderId = new ReminderId(command.ReminderId);

        Reminder? reminder = await reminderRepository.GetAsync(reminderId, cancellationToken);

        if (reminder is null || reminder.WeddingId != new WeddingId(command.WeddingId))
        {
            return Result.Failure(ReminderErrors.NotFound(reminderId));
        }

        reminderRepository.Delete(reminder);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
