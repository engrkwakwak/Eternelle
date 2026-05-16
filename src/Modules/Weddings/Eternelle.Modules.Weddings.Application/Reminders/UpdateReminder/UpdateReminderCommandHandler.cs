using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.Reminders;
using Eternelle.Modules.Weddings.Domain.Shared;

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

        Result<IconIdentifier> iconResult = IconIdentifier.Create(command.Icon);
        if (iconResult.IsFailure)
        {
            return Result.Failure(iconResult.Error);
        }

        Result<ActivityName> titleResult = ActivityName.Create(command.Title);
        if (titleResult.IsFailure)
        {
            return Result.Failure(titleResult.Error);
        }

        Result<RichDescription> bodyResult = RichDescription.Create(command.Body);
        if (bodyResult.IsFailure)
        {
            return Result.Failure(bodyResult.Error);
        }

        reminder.Update(iconResult.Value, titleResult.Value, bodyResult.Value);

        reminderRepository.Update(reminder);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
