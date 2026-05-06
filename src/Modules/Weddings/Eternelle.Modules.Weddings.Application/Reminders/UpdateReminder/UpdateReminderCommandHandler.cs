using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.Reminders;

namespace Eternelle.Modules.Weddings.Application.Reminders.UpdateReminder;

internal sealed class UpdateReminderCommandHandler(
    IReminderRepository reminderRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateReminderCommand>
{
    public async Task<Result> Handle(
        UpdateReminderCommand command,
        CancellationToken cancellationToken)
    {
        var reminderId = new ReminderId(command.ReminderId);

        Reminder? reminder = await reminderRepository.GetAsync(reminderId, cancellationToken);

        if (reminder is null)
        {
            return Result.Failure(ReminderErrors.NotFound(reminderId));
        }

        reminder.Update(command.Icon, command.Title, command.Body);

        reminderRepository.Update(reminder);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
