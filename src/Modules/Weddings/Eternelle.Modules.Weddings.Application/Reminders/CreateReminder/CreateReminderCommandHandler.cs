using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.Reminders;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.Reminders.CreateReminder;

internal sealed class CreateReminderCommandHandler(
    IReminderRepository reminderRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateReminderCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateReminderCommand command,
        CancellationToken cancellationToken)
    {
        var weddingId = new WeddingId(command.WeddingId);

        IReadOnlyList<Reminder> existingReminders = await reminderRepository.GetByWeddingIdAsync(
            weddingId,
            cancellationToken);

        int displayOrder = existingReminders.Count == 0
            ? 0
            : existingReminders.Max(r => r.DisplayOrder) + 1;

        var reminder = Reminder.Create(
            weddingId,
            command.Icon,
            command.Title,
            command.Body,
            displayOrder);

        reminderRepository.Insert(reminder);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(reminder.Id.Value);
    }
}
